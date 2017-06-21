using System;
using UnityEngine;
using System.Collections.Generic;
using Core;

namespace UI.Controls
{

	public class MGList:MonoBehaviour
	{
		[SerializeField]
		protected tk2dUIScrollableArea _scrollableArea;
				
		/*[SerializeField]
		private MGListDataModel _model;*/
		
		[SerializeField]
		private MonoBehaviour _controller;
		
		private readonly Queue<MonoBehaviour> _rows;
		
		private bool _needToRedraw;
		
		private int _rowsOnScreen;
		
		//private float _currentOffset;
				
		private int _poolSize;
		
		[SerializeField]
		private int _currentIndex;
		
		private MGListController ListController
		{
			get
			{
				return (MGListController)_controller;
			}
		}
		
		public MGList ()
		{
			_rows = new Queue<MonoBehaviour>();	
		}
		
		private void Start()
		{
			if(_controller != null)
			{
				CalculateDimenstions();
				_needToRedraw = true;
			}
			else
				MonoLog.Log(MonoLogChannel.UI, "Please define controller");
			
			_scrollableArea.OnScroll += OnScroll;				
		}
		
		protected virtual float GetPosition(Vector3 position)
		{
			if(_scrollableArea.scrollAxes == tk2dUIScrollableArea.Axes.YAxis)
				return position.y;
			
			return position.x;
		}
		
		protected virtual Vector3 SetPosition(Vector3 v3, float position)
		{
			if(_scrollableArea.scrollAxes == tk2dUIScrollableArea.Axes.YAxis)			
				return new Vector3(0f,position,v3.z);
			
			return new Vector3(position,0f,v3.z);
		}
		
		private void CalculateDimenstions()
		{
			_scrollableArea.ContentLength = ListController.GetListRowCount(this) * ListController.GetListRowHeight(this);
			
			_rowsOnScreen = Mathf.CeilToInt(_scrollableArea.VisibleAreaLength / ListController.GetListRowHeight(this));
			
			_poolSize = Math.Min(ListController.GetListRowCount(this), _rowsOnScreen + 2);
			
			/*MonoLog.Log(MonoLogChannel.UI, "Rows on screen: " + _rowsOnScreen);
			MonoLog.Log(MonoLogChannel.UI, "Scrollable content length: " + _scrollableArea.ContentLength);
			MonoLog.Log(MonoLogChannel.UI, "Pool size:" + _poolSize);*/
		}
		
		private void OnScroll(tk2dUIScrollableArea scrollableArea)
		{
			
/*			float scrollOffset = GetPosition( scrollableArea.ContentContainerOffset );
						
			if(Mathf.Abs(_currentOffset - scrollOffset) > 0.01)
			{
				MonoLog.Log(MonoLogChannel.All,"Changes: " + scrollOffset + " --- " + _currentOffset);
				_needToRedraw = true;
			}
			*/
			
			if(CalculateCurrentIndex() != _currentIndex)
			{
				//MonoLog.Log(MonoLogChannel.All,"Changes: " + scrollOffset + " --- " + _currentOffset);
				
				_needToRedraw = true;
			}
			
		}
		
		private int CalculateCurrentIndex()
		{
			return Mathf.FloorToInt( GetPosition( _scrollableArea.ContentContainerOffset ) / ListController.GetListRowHeight(this) );
		}
		
		private int ConvertDataIndexToScreenIndex(int dataIndex)
		{
			return Mathf.FloorToInt( dataIndex / ListController.GetListRowHeight(this) ); // use scroll position
		}
		
		private void Update()
		{
			if( _needToRedraw )
			{
				//MonoLog.Log(MonoLogChannel.UI,"Redraw list");

				_needToRedraw = false;
				
				if(_controller == null)
				{
					MonoLog.Log(MonoLogChannel.UI,"Controller for list does not assigned");
					return;
				}
				
				//_currentOffset = GetPosition(_scrollableArea.ContentContainerOffset);
								
				int startIndex = _currentIndex = CalculateCurrentIndex();

				if(startIndex >= 0)
				{				
					startIndex = Mathf.Max(startIndex - 1, 0);
					
					int endIndex = Mathf.Min(ListController.GetListRowCount(this), startIndex + _poolSize);
					int rowsProcessed = 0;
					
					for(int i = startIndex; i < endIndex; i++)
					{
						MonoBehaviour row = ListController.GetListRow( this, i );
						
						row.transform.parent = _scrollableArea.contentContainer.transform;
						row.transform.localPosition = Vector3.zero;
						
						float p =  i * ListController.GetListRowHeight(this);	
						if(_scrollableArea.scrollAxes == tk2dUIScrollableArea.Axes.YAxis)
							p *= -1;
						
						//p += GetPosition(_cameraSize) - ListController.GetListRowHeight(this) /2;
						//row.transform.localPosition =  new Vector3(0, y ,row.transform.localPosition.z);	
						
						row.transform.localPosition = SetPosition(row.transform.localPosition, p);
						row.gameObject.SetActive(true);
						
						_rows.Enqueue(row);	
						
						rowsProcessed++;
					}
					
					while(rowsProcessed < _poolSize)
					{
						rowsProcessed++;
						
						MonoBehaviour row = _rows.Dequeue();
						
						row.gameObject.SetActive(false);
						
						_rows.Enqueue(row);	

					}
				}
				
				while(_rows.Count > _poolSize)
				{
					//MonoLog.Log(MonoLogChannel.All,"Our rows:" + _rows.Count + ", need just " + ListController.GetListRowCount(this) + ". Rows on screen:" + _rowsOnScreen);
					
					int needToDeleteCount = _rows.Count - _poolSize;
					
					while(needToDeleteCount-- > 0)
					{
						UnityEngine.Object.Destroy( _rows.Dequeue().gameObject );
					}
				}
			}
			
			
		}
		
		public MonoBehaviour DequeueRow()
		{

			if(_rows.Count >= _poolSize)
				return _rows.Dequeue();
			
			return null;
		}
				
		private void OnDestroy()
		{
			_rows.Clear();
			_scrollableArea.OnScroll -= OnScroll;	
		}
		
		public void InvalidateData()
        {
			CalculateDimenstions();
            _needToRedraw = true;
        }
	}	
	
	public interface MGListController
	{
		MonoBehaviour GetListRow(MGList list, int rowIndex);
		
		float GetListRowHeight(MGList list);
		
		int GetListRowCount(MGList list);
	}	
	
}

