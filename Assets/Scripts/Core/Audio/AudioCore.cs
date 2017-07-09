
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Audio
{
    public class AudioCore
    {
        private readonly List<AudioChannel> _channels = new List<AudioChannel>();
        private Dictionary<AudioType, bool> _mute;
        private Dictionary<AudioType, float> _volume;

        public void Init(Transform transform, int countChannels)
        {
            _mute = new Dictionary<AudioType, bool>();
            _volume = new Dictionary<AudioType, float>();

            for (int i = 0; i < countChannels; i++)
            {
                var gameObject = new GameObject("AudioChannel");
                var audioObject = gameObject.AddComponent<AudioChannel>();
                gameObject.transform.SetParent(transform);
                _channels.Add(audioObject);
            }
        }

        public AudioSource Play(SoundSettings soundSettings, AudioClip audioClip, Action onPlayComplete = null, Transform positionObjects = null)
        {
            soundSettings.AudioClip = audioClip;
            return Play(soundSettings, onPlayComplete, positionObjects);
        }

        public AudioSource Play(SoundSettings soundSettings, Action onPlayComplete = null, Transform positionObjects = null)
        {
            if (soundSettings == null)
            {
                Debug.LogError("SoundSettings is null");
                return null;
            }
            if (soundSettings.AudioClip == null && soundSettings.AudioClips.Count <= 0)
            {
                Debug.Log("No audio clip(s) attached to SoundSettings");
                return null;
            }

            var audioChannel = GetAvailableChannel(soundSettings);

            if (audioChannel == null)
                return null;

            audioChannel.PlayAudio(soundSettings, onPlayComplete, positionObjects);

            return audioChannel.AudioSource;
        }

        public void Stop(AudioSource audioSource, float fadeOut = 0)
        {
            if (audioSource == null)
            {
                return;
            }

            var channel = _channels.Find(a => a.AudioSource.Equals(audioSource) && a.SoundSettings != null);
            if (channel != null)
                channel.StopAudio(fadeOut);
        }

        public void Pause(string audioName)
        {
            var channel = GetChannel(audioName);
            if (channel != null)
            {
                channel.Pause = true;
            }
        }

        public void UnPause(string audioName)
        {
            var channel = GetChannel(audioName);
            if (channel != null)
            {
                channel.Pause = false;
            }
        }

        public bool GetMuteByType(AudioType type)
        {
            if (_mute.ContainsKey(type))
                return _mute[type];

            var key = type.ToString();
            if (PlayerPrefs.HasKey(key))
            {
                var value = Convert.ToBoolean(PlayerPrefs.GetInt(key));
                _mute.Add(type, value);
                return value;
            }
            return false;
        }

        public void SetMuteByType(AudioType type, bool value)
        {
            if (_mute.ContainsKey(type))
                _mute[type] = value;
            else
                _mute.Add(type, value);

            for (int i = 0; i < _channels.Count; i++)
            {
                _channels[i].SetMute(type, value);
            }

            PlayerPrefs.SetInt(type.ToString(), Convert.ToInt32(value));
        }

        private AudioChannel GetAvailableChannel(SoundSettings currentSoundSettings)
        {
            var currentPriority = currentSoundSettings.Priority;
            AudioChannel selectedChannel = null;
            for (int i = 0; i < _channels.Count; i++)
            {
                var channel = _channels[i];
                var soundSettings = channel.SoundSettings;

                if (soundSettings == null)
                {
                    if (selectedChannel == null)
                        selectedChannel = channel;
                    continue;
                }

                if (soundSettings.Interrupted)
                {
                    if (currentPriority > 0 && currentPriority.Equals(soundSettings.Priority))
                    {
                        selectedChannel = channel;
                        break;
                    }

                    if (currentPriority == 0 && soundSettings.name.Equals(currentSoundSettings.name))
                    {
                        selectedChannel = channel;
                        break;
                    }
                }
                else
                {
                    if (soundSettings.name.Equals(currentSoundSettings.name))
                    {
                        //new sound will not play if the same already playing
                        if (Time.unscaledTime - channel.TimeStartPlay < soundSettings.MinSecondsDiff)
                        {
                            return null;
                        }
                    }
                }
            }

            return selectedChannel;
        }

        private AudioChannel GetChannel(string soundName)
        {
            for (var i = 0; i < _channels.Count; i++)
            {
                var audioObject = _channels[i];
                var soundSettings = audioObject.SoundSettings;
                if (soundSettings != null && soundSettings.name.Equals(soundName))
                {
                    return audioObject;
                }
            }
            return null;
        }

        public float GetVolumeByType(AudioType type)
        {
            if (_volume.ContainsKey(type))
                return _volume[type];

            return 1f;
        }

        public void SetVolumeByType(AudioType type, float value)
        {
            if (_volume.ContainsKey(type))
                _volume[type] = value;
            else
                _volume.Add(type, value);

            for (var i = 0; i < _channels.Count; i++)
            {
                _channels[i].UpdateGlobalVolume(type);
            }
        }
    }
}
