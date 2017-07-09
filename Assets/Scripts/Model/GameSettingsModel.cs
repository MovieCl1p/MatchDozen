using Commands;
using UnityEngine;

namespace Model
{
    public class GameSettingsModel
    {
        public QualityPreset QualityPreset
        {
            get
            {
                return (QualityPreset)PlayerPrefs.GetInt("QualityPreset", (int)QualityPreset.High);
            }

            set
            {
                PlayerPrefs.SetInt("QualityPreset", (int)value);
            }
        }
    }
}