using Core.Domain;

namespace App.States
{	
	public sealed class AppInitializeState : AppState
	{
		public AppInitializeState ()
		{
		}
		
		protected override void OnStart (object[] args)
		{
			base.OnStart(args);

			AppController.Instance.LoadConfigs();
            
//			if(AppModel.Instance.Player == null)
//			{
//				AppModel.Instance.Player = Player.Default();
//			}
			
			FinishCommand();		
		}
		
	}
}

