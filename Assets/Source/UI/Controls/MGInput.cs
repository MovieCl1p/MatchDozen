using System;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace UI
{
	public sealed class MGInput : MonoBehaviour
	{
		public event Action<Vector3> PressUp;
		public event Action<Vector3> PressDown;
		public event Action<Vector3> Tap;
		
		public event Action<MGComponent> DragStart;
		public event Action<MGComponent,MGComponent> DragDrop;
		public event Action<MGComponent,MGComponent> DragOver;
		public event Action<MGComponent,MGComponent> DragOut;

		
		
		
		public const float SWIPE_SENSIVITY = 0.1f;
		public const float TAP_SENSIVITY = 0.1f;
		
		
		private bool _pressedDown;
		private bool _dragStarted;
		private bool _swipeStarted;
		private Vector3 _lastMousePosition;
		private Vector3 _touchInside;	
		private Vector3 _initialPressPosition;
		private float _touchStartTime;
		private float _tapStartTime;
		
		private float _nextSwipeTime;
		private int _tapCount;
		
		private List<MGComponent> _controlsOnScene;
		
		private List<MGComponent> _lastOverComponents;
		private MGComponent _lastPressedComponent;
		
		private Camera _camera;
		private Transform _transform;
		
		public MGInput ()
		{
			_controlsOnScene = new List<MGComponent>();
			_lastOverComponents = new List<MGComponent>();
		}
		
		private MGComponent ComponentUnderTouch(Vector3 mousePosition, bool ignoreTouch, bool inputEnabledOnly, bool draggableOnly)
		{			
			List<MGComponent> components = new List<MGComponent>();
            
			for(int i = _controlsOnScene.Count-1; i >= 0; i--)
			{
				MGComponent component = _controlsOnScene[i];
				

				if(ignoreTouch)
				{
					if(component == _lastPressedComponent)		
					{
						continue;
					}
				}
				
				
				if(inputEnabledOnly && !component.InputEnabled)
					continue;
				
				if(draggableOnly && !component.Dragable)
					continue;
								
				if(component.HitTest(mousePosition))
					components.Add(component);					
				
			}
			
			if(components.Count > 1)
			{
				components.Sort(delegate(MGComponent x, MGComponent y)
				{					
					return x.CachedTransform.position.z.CompareTo(y.CachedTransform.position.z);	
				});
			}
			
			
			if(components.Count > 0)
				return components[0];
						
			return null;
		}
		
		
		public void StartDrag(MGComponent component,Vector3 mousePosition)
		{					
			if(_lastPressedComponent != null && _lastPressedComponent != component)
			{
				if(_dragStarted)
				{
					FinishDrag();
				}
				
				_lastPressedComponent.OnPressUp();
				_lastPressedComponent = null;
			}			
			
			if(_lastPressedComponent == null)
			{
				component.OnPressDown();
				_lastPressedComponent = component;
				_pressedDown = true;
			}

			
			_dragStarted = true;
						
			/*_touchInside = new Vector3(fingerPosition.x - component.CachedTransform.position.x, 
											   fingerPosition.y - component.CachedTransform.position.y,
											   component.CachedTransform.position.z);*/
																	
			component.OnDragStart(mousePosition);	
			
			if(DragStart != null)
				DragStart(component);
		}
		
		private void Update()
		{						
			if(Input.GetMouseButton(0) && !_pressedDown) // multitouch not supported
			{
				//Vector3 fingerPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);				
				
				_lastMousePosition = Input.mousePosition;
				_initialPressPosition = Input.mousePosition;
				_touchStartTime = Time.time;
			
				_pressedDown = true;
				
				if(_lastPressedComponent != null)
				{
					if(!_lastPressedComponent.HitTest(Input.mousePosition))
					{
						_lastPressedComponent.OnPressUp();
						_lastPressedComponent = null;
						_lastOverComponents.Clear();					
					}
				}
				
				if(_lastPressedComponent == null)
				{
					_lastPressedComponent = ComponentUnderTouch(Input.mousePosition,false,true,false);
					
					if(_lastPressedComponent != null)
						_lastPressedComponent.OnPressDown();
				}
				
				if(PressDown != null)
					PressDown(Input.mousePosition);
			}
			
			
			if(_pressedDown)
			{
				if(Input.GetMouseButton(0))
				{	
					if(_lastPressedComponent != null)
					{
						if(_lastMousePosition != Input.mousePosition)
						{	
							if(!_swipeStarted && _lastPressedComponent.Dragable)
							{
								Vector3 fingerPosition = _lastPressedComponent.Camera.ScreenToWorldPoint(Input.mousePosition);
								
								if(!_dragStarted && (_touchStartTime + _lastPressedComponent.DragDelay) <= Time.time)
								{							
									StartDrag(_lastPressedComponent, Input.mousePosition);
								}
								
								if(_dragStarted)
								{											
									_lastPressedComponent.CachedTransform.position = new Vector3(fingerPosition.x - _lastPressedComponent.TouchInside.x, 
										fingerPosition.y - _lastPressedComponent.TouchInside.y,
										_lastPressedComponent.CachedTransform.position.z);									
		
									MGComponent overComponent = ComponentUnderTouch(Input.mousePosition,true,false,false);
									
									
									for(int i = _lastOverComponents.Count-1; i >= 0; i--)
									{
										MGComponent lastOverComponent = _lastOverComponents[i];
										
										if(lastOverComponent == null)
										{
											_lastOverComponents.RemoveAt( i );
											continue;
										}
										
										if(!lastOverComponent.HitTest( Input.mousePosition ))
										{
											_lastPressedComponent.OnDragOut(lastOverComponent);	
											
											if(DragOut != null)
												DragOut(_lastPressedComponent,lastOverComponent);
											
											_lastOverComponents.RemoveAt( i );
										}
									}
							
									if(overComponent != null && !_lastOverComponents.Contains(overComponent))
									{	
										_lastPressedComponent.OnDragOver(overComponent);	
										_lastOverComponents.Add(overComponent);
										
										if(DragOver != null)
											DragOver(_lastPressedComponent,overComponent);
									}								
								}
							}						
																
							_lastMousePosition = Input.mousePosition;
							
							if(!_dragStarted)
							{
								_swipeStarted = true;
							}
							
							if(_swipeStarted && _nextSwipeTime < Time.time)
							{
								_nextSwipeTime = Time.time + SWIPE_SENSIVITY;
                                //TODO: old code: Camera.main; check it!
								_lastPressedComponent.OnSwipe(Camera.allCameras[0].ScreenToWorldPoint(_lastMousePosition));
							}						
					   }
					   else if(_lastPressedComponent.Dragable 
							&& !_dragStarted 
							&& !_swipeStarted 
							&& _lastPressedComponent.DragStartAfterDelay 
							&& (_touchStartTime+_lastPressedComponent.DragDelay) <= Time.time)
					   {						
							StartDrag(_lastPressedComponent,Input.mousePosition);					
					   }
					}
				}
				else
				{	
					if(_lastPressedComponent != null)
					{
						if(_lastPressedComponent.HitTest(_lastMousePosition))
						{
                            //TODO: old code: Camera.main; check it!
                            _lastPressedComponent.OnTap(Camera.allCameras[0].ScreenToWorldPoint(_lastMousePosition));	
						}
																					
						if(_dragStarted)
						{	
							FinishDrag();						
						}

						_lastPressedComponent.OnPressUp();	
					}
					
					_lastPressedComponent = null;						
					_pressedDown = _swipeStarted = _dragStarted = false;	
					_lastOverComponents.Clear();	
					
					if(PressUp != null)
						PressUp( Input.mousePosition );
					
					if(Tap != null && _initialPressPosition == _lastMousePosition)
						Tap( Input.mousePosition );
						
				}
			}						
			
		}
		
		private void FinishDrag()
		{
			MGComponent lastOverComponent = null;
			
			if(_lastOverComponents.Count > 0)
			{	
				for(int i = _lastOverComponents.Count-1; i >= 0; i--)
				{
					_lastPressedComponent.OnDragOut(_lastOverComponents[i]);	
					
					if(DragOut != null)
						DragOut(_lastPressedComponent,lastOverComponent);					
				}
				
				lastOverComponent = _lastOverComponents[_lastOverComponents.Count-1];				
			}
					
			if(DragDrop != null)
				DragDrop(_lastPressedComponent,lastOverComponent);
			
			_lastPressedComponent.OnDragDrop(lastOverComponent);													
						
			_lastOverComponents.Clear();			
		}
		
		internal void Add(MGComponent component)
		{
			_controlsOnScene.Add(component);
		}
		
		internal void Remove(MGComponent component)
		{
			_controlsOnScene.Remove(component);
		}
	}
}

