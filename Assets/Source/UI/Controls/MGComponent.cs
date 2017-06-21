using Core;
using UnityEngine;

namespace UI
{	
	public abstract class MGComponent : MonoScheduledBehaviour
	{			
		public const float SWIPE_SENSIVITY = 0.05f;
		
		public event System.Action<MGComponent> PressDown;
		public event System.Action<MGComponent> PressUp;
		public event System.Action<MGComponent> DragStart;
		public event System.Action<MGComponent,MGComponent> DragOver;
		public event System.Action<MGComponent,MGComponent> DragOut;
		public event System.Action<MGComponent,MGComponent> DragDrop;
		
		public event System.Action<MGComponent,Vector3> Tap;
		public event System.Action<MGComponent,Vector3> Swipe;
		
		private Vector3 _touchInside;
		
		public Vector3 TouchInside
		{
			get
			{
				return _touchInside;
			}
		}
		
		[SerializeField]
		private float _width;
		
		[SerializeField]
		private float _height;
		
		[SerializeField]		
		private bool _inputEnabled;		
		
		private Camera _camera;
		
		private bool _inputRegistered;
		

		
		public static void ChangeLayer(GameObject gameObject, int layer)
		{
			gameObject.layer = layer;
			
			if(gameObject.transform.childCount > 0)
			{
				for(int i = 0; i < gameObject.transform.childCount; i++)
				{
					ChangeLayer(gameObject.transform.GetChild(i).gameObject, layer);
				}
			}
		}		
		
		static public Camera FindCameraForLayer (int layer)
		{
			int layerMask = 1 << layer;
	
			Camera[] cameras = (Camera[])GameObject.FindObjectsOfType(typeof(Camera));
	
			for (int i = 0, imax = cameras.Length; i < imax; ++i)
			{
				Camera cam = cameras[i];
	
				if ((cam.cullingMask & layerMask) != 0)
				{
					return cam;
				}
			}
			
			return null;
		}		
		
		public Camera Camera
		{
			get
			{
				if(_camera == null)
					_camera = FindCameraForLayer(gameObject.layer);
				
				return _camera;
			}
		}
		
		public int Layer
		{
			set
			{
				if(value != gameObject.layer)
				{
					ChangeLayer(gameObject, value);
					
					_camera = FindCameraForLayer(gameObject.layer);					
				}
			}
		}
					
		public float Width
		{
			get
			{
				return _width;
			}
			set
			{
				_width = value;				
			}
		}
		
		
		public float Height
		{
			get
			{
				return _height;
			}
			set
			{
				_height = value;
			}
		}
		
		public bool InputEnabled
		{
			get
			{
				return _inputEnabled;
			}
			set
			{
				_inputEnabled = value;
			}
		}
		
		protected override sealed void Start()
		{
			RegisterInput();
			
			OnStart();
		}
		
		protected virtual void OnStart()
		{
			
		}
		
		private void RegisterInput()
		{
			if(!_inputRegistered && UIManager.CurrentSceneController != null)
			{
				UIManager.CurrentSceneController.SceneInput.Add(this);
				_inputRegistered = true;
			}
		}
		
		private void UnregisterInput()
		{
			if(_inputRegistered)
			{
				_inputRegistered = false;
				
				if(UIManager.CurrentSceneController != null)
					UIManager.CurrentSceneController.SceneInput.Remove(this);
			}
		}
		
		protected virtual void OnEnable()
		{
			RegisterInput();
		}
		
		protected virtual void OnDisable()
		{
			UnregisterInput();
		}
		
		protected override void OnDestroy()
		{
			base.OnDestroy();
			
			UnregisterInput();
		}
				
		public bool Dragable;
		
		public bool DragStartAfterDelay
		{
			get;set;
		}	
		
		public float DragDelay;
		
		public MGComponent ()
		{
			InputEnabled = true;
		}
		
		public virtual bool HitTest(Vector3 mousePosition)
		{
		    if (Camera != null)
		    {
                return CreateRect().Contains(Camera.ScreenToWorldPoint(mousePosition));
		    }
			
			return false;
		}
		
		public virtual Rect CreateRect()
		{
			return new Rect(CachedTransform.position.x - (Width * Mathf.Abs(CachedTransform.localScale.x)) /2,
							CachedTransform.position.y, 
							Width * Mathf.Abs( CachedTransform.localScale.x ), 
							Height * CachedTransform.localScale.y);					
		}
		
		/*public void StartDrag(Vector3 fingerPosition)
		{
			_dragStarted = _pressedDown = true;
						
			_touchInside = new Vector3(fingerPosition.x - CachedTransform.position.x, 
											   fingerPosition.y - CachedTransform.position.y,
											   CachedTransform.position.z);
										
			_controlsOnScene = new List<object>(GameObject.FindSceneObjectsOfType(typeof(MGComponent)));				
				
			_controlsOnScene.Remove(this);	
			
			if( DragStart != null)
				DragStart(this);															
		}*/
		
		internal void OnPressUp()
		{
			if(PressUp != null)
				PressUp(this);
		}
		
		internal void OnPressDown()
		{
			if(PressDown != null)
				PressDown(this);
		}
		
		internal void OnDragStart(Vector3 mousePosition)
		{
			Vector3 fingerPosition = Camera.ScreenToWorldPoint(mousePosition);
			
 			_touchInside = new Vector3(fingerPosition.x - CachedTransform.position.x, 
											   fingerPosition.y - CachedTransform.position.y,
											   CachedTransform.position.z);	
			if(DragStart != null)
				DragStart(this);
		}
		
		internal void OnDragDrop(MGComponent component)
		{
			if(DragDrop != null)
				DragDrop(this,component);
		}
		
		internal void OnDragOver(MGComponent component)
		{
			if(DragOver != null)
				DragOver(this,component);
		}
		
		internal void OnDragOut(MGComponent component)
		{
			if(DragOut != null)
				DragOut(this,component);
		}
		internal void OnTap(Vector3 pos)
		{
			if(Tap != null)
				Tap(this,pos);
		}
		
		internal void OnSwipe(Vector3 pos)
		{
			if(Swipe != null)
				Swipe(this,pos);
		}
		
		protected virtual void OnUpdate()
		{
			
		}
		
		protected override void Update()
		{			
			base.Update();
			
#if UNITY_EDITOR			
		Rect rect = CreateRect();
			
		Color color = _inputEnabled ? Color.white : Color.red;
			
			if(!_inputRegistered)
				color = Color.magenta;
	
		Debug.DrawLine(new Vector3(rect.xMin,rect.yMax,transform.position.z), new Vector3(rect.xMax,rect.yMax,transform.position.z),color);
		Debug.DrawLine(new Vector3(rect.xMax,rect.yMax,transform.position.z), new Vector3(rect.xMax,rect.yMin,transform.position.z),color);
		Debug.DrawLine(new Vector3(rect.xMax,rect.yMin,transform.position.z), new Vector3(rect.xMin,rect.yMin,transform.position.z),color);
		Debug.DrawLine(new Vector3(rect.xMin,rect.yMin,transform.position.z), new Vector3(rect.xMin,rect.yMax,transform.position.z),color);			
#endif		
			
			OnUpdate();
		}
			
	}
	
	
	abstract public class MGComponentWithModel<T>:MGComponent,Observer
//		where T:Observable
	{
		private T _model;
		
		public virtual T Model
		{
			get
			{
				return _model;
			}
			 set
			{
				if(_model != null && _model is Observable)
					(_model as Observable).RemoveObserver(this);
				
				_model = value;
				
				if(_model != null)
				{
					if(_model is Observable)
						(_model as Observable).AddObserver(this);
				
					ApplyModel();
					
					OnModelChanged();
				}
			}
		}
	
		protected virtual void ApplyModel() {}
		protected virtual void OnModelChanged() {}
		
		#region Observer implementation
		public void OnObjectChanged (Observable observable)
		{
			OnModelChanged();
		}
		#endregion
	}
}

