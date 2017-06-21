using System;

namespace UI
{
	public abstract class UIPopUpSceneController : UISceneController
	{
		public virtual void OnClose()
		{
			UIManager.ClosePopUp(this);
		}
		
		public virtual Object Result
		{
			get
			{
				return String.Empty;
			}
		}
		
		public UIPopUpSceneController ()
		{
		}
	}
}

