using UnityEngine;
using System.Collections.Generic;

namespace UI.Controls
{
	public sealed class MGTable:MonoBehaviour
	{
		[SerializeField]
		private MonoBehaviour _controller;
		
		private bool _needUpdate;
		private Rect _cellRect;
		
		private readonly List<MonoBehaviour> _cells;
		
		
		
		//private Vector3 _cameraSize;

		
		private MGTableListener TableController
		{
			get
			{
				return (MGTableListener)_controller;
			}
		}
				
		public MGTable ()
		{
			_cells = new List<MonoBehaviour>();

		}
		
		private void Start()
		{
			//_cameraSize = tk2dUIManager.Instance.UICamera.ScreenToWorldPoint(new Vector3(tk2dUIManager.Instance.UICamera.pixelWidth, tk2dUIManager.Instance.UICamera.pixelHeight));
			
			if(_controller != null)
			{
				InvalidateData();
			}
		}
		
		public void InvalidateData()
		{
			_cellRect = TableController.GetTableCellRect(this);
			_needUpdate = true;
		}
		
		void Update()
		{
			if(_needUpdate)
			{
				_needUpdate = false;
				
				int rowIndex = 0;
				int columnIndex = 0;
				
				for(int i = 0; i < TableController.GetCellsCount(this); i++)
				{					
					if(TableController.GetColumnsCount(this) == columnIndex)
					{
						rowIndex++;
						columnIndex = 0;
					}
					
					MonoBehaviour cell = TableController.GetTableCell(this,rowIndex,columnIndex,i);
					
					cell.transform.parent = this.transform;
					
					float x = columnIndex * (_cellRect.width + TableController.GetCellPadding(this));
					float y = - rowIndex * (_cellRect.height + TableController.GetCellPadding(this));
					y -= _cellRect.height/2;
					x += _cellRect.width/2;
					
					/*y += TableController.GetCellPadding(this);					
					x += TableController.GetCellPadding(this);
					
					if(rowIndex > 0)
						y += TableController.GetCellPadding(this);					
					
					if(columnIndex > 0)
						x += TableController.GetCellPadding(this);*/

					//y += tk2dUIManager.Instance.UICamera.transform.position.y;
					
					//y *= -1;				
					///y += _cameraSize.y;
					
					
					//x -= _cameraSize.x - _cellRect.width/2;

					cell.transform.position = new Vector3(x , y , cell.transform.position.z);
					cell.transform.position += new Vector3(transform.position.x, transform.position.y, 0);
					cell.gameObject.SetActive(true);
					
					if(_cells.Count <= i)
						_cells.Add( cell );
					
					columnIndex++;
				}
				
				if(_cells.Count > TableController.GetCellsCount(this))
				{
					int startIndex = TableController.GetCellsCount(this);
					
					for(int i = startIndex; i < _cells.Count; i++)
					{
						_cells[i].gameObject.SetActive(false);
					}
				}
			}
		}
		
		public MonoBehaviour GetCell(int index)
		{
			if(_cells.Count > index)
				return _cells[index];
			
			/*if(_cells.Count >= TableController.GetCellsCount(this))
				return _cells.Dequeue();*/
			
			return null;
		}		
	}
		
	public interface MGTableListener
	{
		//int GetRowsCount(MGTable table);
		int GetColumnsCount(MGTable table);
		int GetCellsCount(MGTable table);	
		float GetCellPadding(MGTable table);
		
		MonoBehaviour GetTableCell(MGTable table, int row, int column, int cellIndex);
						
		Rect GetTableCellRect(MGTable table);
	}
}

