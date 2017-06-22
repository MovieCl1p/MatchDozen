using System;
using Config;
using Core;

namespace App
{
	public static class AppConfig
	{
		public const Boolean RuntimeSaveConfig = true;
        
		public static GameConfig Game
		{
			get;
			private set;
		}
        
		public static LevelConfig Levels
		{
			get;
			private set;
		}

		public static void LoadDefault(AppModel model)
		{
//			Localization = LocalizationConfig.Default(model.Language);
//			Game = GameConfig.Default();
//			GameCenter = GameCenterConfig.Default();
//			Elements = ElementConfig.Default();
			Levels = LevelConfig.Default();
            Game = GameConfig.Default();
        }
		
		public static bool Load(AppModel model)
		{
			try
			{
//				Localization = LocalizationConfig.Load(model.Language);	
//				Game = GameConfig.Default();//GameConfig.Load(GameConfig.FILE_NAME);
//				GameCenter = GameCenterConfig.Default();//GameCenterConfig.Load(GameCenterConfig.FILE_NAME);
//				Elements = ElementConfig.Default();
				Levels = LevelConfig.Default();
			    Game = GameConfig.Default();
			}
			catch(Exception e)
			{				
				MonoLog.Log(MonoLogChannel.Exceptions, e);
				
				return false;
			}
			
			return true;
		}
	}
}