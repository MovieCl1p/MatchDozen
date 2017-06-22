using System;
using UnityEngine;

namespace UI
{
	internal sealed class UIPopUpGlass:MGComponent
	{
		protected override void OnStart ()
		{
			base.OnStart ();
			Width = Height = 10;
			
			BoxCollider mCollider = this.gameObject.AddComponent<BoxCollider>();
			Vector2 cameraSize = GetMainCameraSize();
			mCollider.size = new Vector3(cameraSize.x, cameraSize.y, 0.03f);
			mCollider.center = new Vector3(mCollider.center.x, mCollider.center.y, 1f);
		}
		
		public override bool HitTest (UnityEngine.Vector3 mousePosition)
		{
			return true;
		} 
		
		public override UnityEngine.Rect CreateRect()
		{
			return new UnityEngine.Rect(transform.position.x - Width/2,
											transform.position.y - Height/2, 
											Width, 
											Height);					
		}
		
		private Vector2 GetMainCameraSize()
		{
			Vector3 bottomLeftPoint = Camera.main.ScreenToWorldPoint(Vector3.zero);
			Vector3 topLeftPoint = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0));
			Vector3 bottomRightPoint = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0));
			
			return new Vector2(Math.Abs(bottomRightPoint.x - bottomLeftPoint.x), Math.Abs(topLeftPoint.y - bottomLeftPoint.y));
		}
	}
				
	public interface IUIPopUpControllerHandler
	{
		void PopUpDidFinish(UIPopUpController controller);
	}
	

	internal sealed class UIPopUpSceneVirtualParent:UIPopUpController
	{
		private readonly System.Object _result;
		
	 	public UIPopUpSceneVirtualParent(System.Object result)
		{
			_result = result;
		}		
		
		public override object Result
		{
			get 
			{
				return _result;
			}
		}
	}
	
	/*public abstract class UIPopUpControllerHandler<T>:UIController, IUIPopUpControllerHandler
		where T:UIPopUpControllerHandler<T>
	{
		
		public void PopUpDidFinish(UIPopUpController controller)
		{
			OnPopUpDidFinish((T)controller);
		}
		
		protected abstract void OnPopUpDidFinish(T controller);
	}*/
	
	public abstract class UIPopUpController:UIController
	{
		private bool _closed;

		public const float FIXED_SORTING_POSITION = -5f;
		
		public bool IsCanceled
		{
			get;
			protected set;
		}
		
		public event Action<UIPopUpController> Closed;

		public UIPopUpController ()
		{
		}	
		
		protected override void OnInitialize ()
		{
			base.OnInitialize ();
			
			Vector3 position = gameObject.transform.position;
			position.z = FIXED_SORTING_POSITION;
			gameObject.transform.position = position;
			
			this.gameObject.AddComponent<UIPopUpGlass>();
		}
		
		protected virtual void OnClose()
		{
			Close();
		}
		
		protected virtual void OnCancel()
		{
			IsCanceled = true;
			Close();
		}
		
		public virtual void Close()
		{
			if(!_closed)
			{
				_closed = true;

				UIManager.ClosePopUp(this);
			
				if( Closed != null )
					Closed(this);			
			}
		}
		
		protected sealed override void Update ()
		{
			base.Update ();
		}
		
		public virtual System.Object Result
		{
			get
			{
				return String.Empty;
			}
		}
		
		internal IUIPopUpControllerHandler Handler
		{
			get;set;
		}
	}
}

