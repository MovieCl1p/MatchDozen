using Core.Utils;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using System.IO;
using UnityEngine;

namespace Commands.Startup
{
    public class InitializeSettingsCommand : Command
    {
        [Inject(ContextKeys.CONTEXT_DISPATCHER)]
        public IEventDispatcher GlobalDispatcher { get; set; }
        
        private const string SettingsFileName = "settings.json";

        private DirectoryInfo _directoryInfo;
        //private SettingsResponseDto _localSettings;

        public override void Execute()
        {
            Retain();

            var directoryPath = FileUtils.GetPersistandPath();
            bool isSettingsFolderAvailable = !string.IsNullOrEmpty(directoryPath);
            string localData;

            Debug.Log("newPath: " + directoryPath);
            Debug.Log("persistentDataPath: " + Application.persistentDataPath);

            if (isSettingsFolderAvailable)
            {
                _directoryInfo = new DirectoryInfo(directoryPath);
                localData = FileUtils.ReadFile(_directoryInfo, SettingsFileName);
                
                bool isNeedRefreshSettings = (localData == null || !PlayerPrefs.GetString("v").Equals(Application.version) || Application.isEditor);
                if (isNeedRefreshSettings)
                {
                    var embededData = Resources.Load<TextAsset>("settings");
                    localData = embededData.text;

                    File.WriteAllText(GetPersistentFilePath(), localData);
                    PlayerPrefs.SetString("v", Application.version);
                }
            }
            else
            {
                var embededData = Resources.Load<TextAsset>("settings");
                localData = embededData.text;
            }
            
            //_localSettings = JsonConvert.DeserializeObject<SettingsResponseDto>(localData);

            if (Application.internetReachability == NetworkReachability.NotReachable || !isSettingsFolderAvailable)
            {
                UpdateData();
                //GlobalDispatcher.Dispatch(PromoEvent.ReadCachedData);
            }
            else
            {
                //TODO: need some class with timeouts
                //TimeoutUtils.AddTimeout(RequestTimeout, 1);
            }
        }

        private void UpdateData()
        {
            //PopulateBank();
            //PopulateBoosters();
            //PopulateItems();
            //PopulateLevelFailed();
            //PopulateGameSettings();
            //PopulateCharacters();
            //PopulateVideoAd();
            //PopulateFortuneWheel();
            //PopulateDaily();

            //PopulateMapsSettings();
            Release();
        }

        private void RequestTimeout()
        {
            Debug.Log("remove settings request by timeout");
            //NetworkService.NetManager.Unsubscribe<SettingsResponseDto>(OnRemoteResponse);
            UpdateData();
            //GlobalDispatcher.Dispatch(PromoEvent.ReadCachedData);
        }

        private string GetPersistentFilePath()
        {
            return Path.Combine(FileUtils.GetPersistandPath(), SettingsFileName);
        }
    }
}
