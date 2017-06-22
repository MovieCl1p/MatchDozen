using System.Runtime.Serialization;
using Configs;
using Core;
using Data.Serialization;

namespace App
{
	public enum DifficultyMode
	{
		Easy,
		Medium,
		Hard
	}
	
	public sealed class AppModel : Config<AppModel, JsonSerializationPolicy>, Observer
    {
		private static AppModel s_instance;
		
		public const string CONFIG_NAME = "AppModel.json";

	    public AppModel(SerializationInfo info, StreamingContext context) : base(CONFIG_NAME)
        {
        }

        public AppModel() : base(CONFIG_NAME)
        {
			s_instance = this;			
		}
		
		public static AppModel Instance
		{
			get
			{
				return s_instance;
			}
		}

	    public void OnObjectChanged(Observable observable)
	    {
            SetChanged();
        }
    }
}
