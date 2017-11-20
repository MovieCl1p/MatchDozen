using Model;
using strange.extensions.command.impl;
using UnityEngine;

namespace Commands
{
    public enum QualityPreset
    {
        High = 0,
        Low = 1
    }

    public class SetQualityCommand : Command
    {
        [Inject]
        public QualityPreset QualityPreset { get; set; }

        [Inject]
        public GameSettingsModel GameSettingsModel { get; set; }

        public override void Execute()
        {
            if ((SystemInfo.deviceModel.ToLower().Contains("samsung") && SystemInfo.deviceModel.ToLower().Contains("i9500")) ||
                SystemInfo.graphicsDeviceName.ToLower().Contains("powervr"))
            {
                QualitySettings.SetQualityLevel((int)QualityPreset.Low);
            }
            else
            {
                QualitySettings.SetQualityLevel((int)QualityPreset);
            }

            GameSettingsModel.QualityPreset = QualityPreset;

            Debug.Log("Quality setted: " + QualityPreset);
        }
    }
}