using System;
using Core.ResourceManager;
using UnityEngine;

namespace Core.Audio
{
    public class AudioManager : MonoBehaviour
    {
        private const string SdkError = "AudioManager is not inititlized";
        [SerializeField]
        private int _maxChannels = 7;

        [SerializeField]
        private AudioListener _baseAudioListener;

        private static AudioListener _baseStaticAudioListener;

        public static string DefaultSection { get; set; }

        private static AudioCore _instance;

        public void Awake()
        {
            if (_instance != null || !Application.isPlaying)
            {
                return;
            }

            _baseStaticAudioListener = _baseAudioListener;

            _instance = new AudioCore();
            _instance.Init(transform, _maxChannels);
            DontDestroyOnLoad(gameObject);
        }

        public static AudioSource Play(string sectionName, string settingsName, Action onComplete = null, Transform positionObjects = null)
        {
            if (_instance == null)
            {
                Debug.Log(SdkError);
                return null;
            }

            var soundSettings = ResourcesCache.GetObject<SoundSettings>(DefaultSection, settingsName);  //TEMPORARY
            return _instance.Play(soundSettings, onComplete, positionObjects);
        }

        public static AudioSource Play(string settingsName, Action onComplete = null, Transform positionObjects = null)
        {
            if (_instance == null)
            {
                Debug.Log(SdkError);
                return null;
            }

            var soundSettings = ResourcesCache.GetObject<SoundSettings>(DefaultSection, settingsName);
            return _instance.Play(soundSettings, onComplete, positionObjects);
        }

        public static AudioSource PlayBySource(string settingsName, string audioClipName, Action onComplete = null, Transform positionObjects = null)
        {
            if (_instance == null)
            {
                Debug.Log(SdkError);
                return null;
            }

            var soundSettings = ResourcesCache.GetObject<SoundSettings>(DefaultSection, settingsName);
            var audioClip = ResourcesCache.GetObject<AudioClip>(DefaultSection, audioClipName);

            if (audioClip == null)
            {
                Debug.Log("AudioManager - No AudioClip with name " + audioClipName);
                return null;
            }

            return _instance.Play(soundSettings, audioClip, onComplete, positionObjects);
        }

        public static AudioSource Play(SoundSettings soundSettings, AudioClip audioClip, Action onComplete = null, Transform positionObjects = null)
        {
            if (_instance == null)
            {
                Debug.Log(SdkError);
                return null;
            }
            return _instance.Play(soundSettings, audioClip, onComplete, positionObjects);
        }

        public static AudioSource Play(SoundSettings soundSettings, Action onComplete = null, Transform positionObjects = null)
        {
            if (_instance == null)
            {
                Debug.Log(SdkError);
                return null;
            }
            return _instance.Play(soundSettings, onComplete, positionObjects);
        }

        public static void Stop(AudioSource audioSource, float fadeOut = 0)
        {
            if (_instance == null)
            {
                Debug.Log(SdkError);
                return;
            }

            _instance.Stop(audioSource, fadeOut);
        }

        public static void Pause(string settingsName)
        {
            if (_instance == null)
            {
                Debug.Log(SdkError);
                return;
            }

            _instance.Pause(settingsName);
        }

        public static void UnPause(string settingsName)
        {
            if (_instance == null)
            {
                Debug.Log(SdkError);
                return;
            }

            _instance.UnPause(settingsName);
        }

        public static bool IsMusicOn
        {
            get
            {
                return !GetMuteByType(AudioType.Music);
            }
            set
            {
                SetMuteByType(AudioType.Music, !value);
            }
        }

        public static bool IsSoundOn
        {
            get
            {
                return !GetMuteByType(AudioType.Sound);
            }
            set
            {
                SetMuteByType(AudioType.Sound, !value);
            }
        }

        public static void SetMuteByType(AudioType type, bool value)
        {
            if (_instance == null)
            {
                Debug.Log(SdkError);
                return;
            }

            _instance.SetMuteByType(type, value);
        }

        public static bool GetMuteByType(AudioType type)
        {
            if (_instance == null)
            {
                Debug.Log(SdkError);
                return true;
            }

            return _instance.GetMuteByType(type);
        }

        public static float GetVolumeByType(AudioType type)
        {
            if (_instance == null)
            {
                Debug.Log(SdkError);
                return 0;
            }

            return _instance.GetVolumeByType(type);
        }


        public static void SetVolumeByType(AudioType type, float value)
        {
            if (_instance == null)
            {
                Debug.Log(SdkError);
                return;
            }

            _instance.SetVolumeByType(type, value);
        }

        public static bool ActiveBaseAudioListener
        {
            set
            {
                _baseStaticAudioListener.enabled = value;
            }
        }
    }
}
