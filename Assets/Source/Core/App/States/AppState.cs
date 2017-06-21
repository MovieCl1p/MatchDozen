using Core;

namespace App.States
{	
	public class AppState:StateCommand
	{
		private AppController _controller;
		
		protected AppController Controller
		{
			get
			{
				return _controller;
			}
		}
		
		public AppState ()
		{
			
		}
		
		
		protected override void OnStart (object[] args)
		{
			base.OnStart(args);
			
			_controller = (AppController)args[0];
		}
	}
}

