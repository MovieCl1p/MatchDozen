
using Core.Utils;
using System;
using System.Collections;
using UnityEngine;

namespace Core.Audio
{
    public class AudioObject
    {
        public SoundSettings SoundSettings;
        public Transform ObjectPosition;
    }

    [RequireComponent(typeof(AudioSource))]
    public class AudioChannel : MonoBehaviour
    {
        [HideInInspector]
        public AudioSource AudioSource;

        public SoundSettings SoundSettings;
        public float TimeStartPlay;

        private bool _isPaused;
        private AudioObject _tmpAudioObject;
        private Action _onPlayComplete;

        private Transform _objectPosition;

        private void Awake()
        {
            AudioSource = GetComponent<AudioSource>();
        }

        public void PlayAudio(SoundSettings soundSettings, Action onPlayComplete, Transform objectPosition = null)
        {
            if (SoundSettings != null)
            {
                if (AudioSource.clip.name.Equals(soundSettings.AudioClip.name))
                {
                    //unpause sound on play
                    Pause = false;
                    return;
                }

                _tmpAudioObject = new AudioObject
                {
                    ObjectPosition = objectPosition,
                    SoundSettings = soundSettings
                };

                _onPlayComplete = onPlayComplete;
                StopAudio(SoundSettings.FadeOut);
                return;
            }

            SetNewSound(soundSettings);

            AudioSource.mute = AudioManager.GetMuteByType(SoundSettings.AudioType);
            TimeStartPlay = Time.unscaledTime;

            SetPosition(objectPosition);

            if (soundSettings.Timeout != 0f)
            {
                //TODO: timeout utils
                //TimeoutUtils.AddTimeout<Action>(Timeout, soundSettings.Timeout, onPlayComplete);
            }
            else
            {
                Timeout(onPlayComplete);
            }
        }

        private void Timeout(Action onPlayComplete)
        {
            if (SoundSettings.FadeIn > 0)
                StartCoroutine(StartFadeIn());

            if (!SoundSettings.Loop)
                StartCoroutine(OnSoundPlayComplete(onPlayComplete));

            AudioSource.Play();
        }

        private void SetPosition(Transform objectPosition)
        {
            if (objectPosition != null)
            {
                _objectPosition = objectPosition;
                AudioSource.spatialBlend = 1f;
                AudioSource.dopplerLevel = 0f;
                AudioSource.rolloffMode = AudioRolloffMode.Linear;
                AudioSource.maxDistance = 15f;
            }
            else if (_objectPosition != null)
            {
                _objectPosition = null;
                transform.position = Vector3.zero;
                AudioSource.spatialBlend = 0f;
            }
        }

        public void StopAudio(float fadeOut)
        {
            StopAllCoroutines();

            if (fadeOut <= 0)
                fadeOut = SoundSettings.FadeOut;

            if (fadeOut > 0)
            {
                StartCoroutine(StartFadeOut(fadeOut));
            }
            else
            {
                OnAudioStopped();
            }
        }

        public void SetMute(AudioType type, bool value)
        {
            if (AudioSource != null && SoundSettings != null)
            {
                if (SoundSettings.AudioType.Equals(type))
                    AudioSource.mute = value;
            }

        }

        public void UpdateGlobalVolume(AudioType type)
        {
            if (AudioSource != null && SoundSettings != null)
            {
                if (SoundSettings.AudioType.Equals(type))
                    Volume = SoundSettings.MaxVolume;
            }
        }

        private void Update()
        {
            if (_objectPosition != null)
            {
                transform.position = _objectPosition.position;
            }
        }

        private IEnumerator OnSoundPlayComplete(Action onComplete)
        {
            yield return new WaitForSecondsRealtime(AudioSource.clip.length);

            SoundSettings = null;

            if (onComplete != null)
                onComplete();
        }

        private IEnumerator StartFadeIn()
        {
            var fadeStart = Time.unscaledTime;

            while (Time.unscaledTime < (fadeStart + SoundSettings.FadeIn))
            {
                Volume = CubicInOut(Time.unscaledTime - fadeStart, 0f, SoundSettings.MaxVolume, SoundSettings.FadeIn);
                yield return null;
            }

            Volume = SoundSettings.MaxVolume;
        }

        private IEnumerator StartFadeOut(float fadeOut)
        {
            float fadeStart = Time.unscaledTime;
            var changeValue = -SoundSettings.MaxVolume;
            while (Time.unscaledTime < (fadeStart + fadeOut))
            {
                Volume = CubicInOut(Time.unscaledTime - fadeStart, SoundSettings.MaxVolume, changeValue, fadeOut);
                yield return null;
            }

            OnAudioStopped();
        }

        private void OnAudioStopped()
        {
            if (AudioSource.isPlaying)
                AudioSource.Stop();

            SoundSettings = null;

            if (_tmpAudioObject != null)
                PlayAudio(_tmpAudioObject.SoundSettings, _onPlayComplete, _tmpAudioObject.ObjectPosition);
        }

        private void SetNewSound(SoundSettings soundSettings)
        {
            _tmpAudioObject = null;
            _onPlayComplete = null;
            _isPaused = false;

            SoundSettings = soundSettings;

            AudioSource.loop = soundSettings.Loop;

            if (soundSettings.AudioClips.Count > 0)
                GetSoundPossibility(soundSettings);
            else
                AudioSource.clip = soundSettings.AudioClip;

            if (SoundSettings.MinPitch >= 0.5f && SoundSettings.MaxPitch <= 2)
                AudioSource.pitch = UnityEngine.Random.Range(SoundSettings.MinPitch, SoundSettings.MaxPitch);
            else
                AudioSource.pitch = 1;

            if (SoundSettings.MinVolume.Equals(soundSettings.MaxVolume))
                Volume = soundSettings.MaxVolume;
            else
                Volume = UnityEngine.Random.Range(SoundSettings.MinVolume, SoundSettings.MaxVolume);

            if (soundSettings.AudioMixerGroup != null)
                AudioSource.outputAudioMixerGroup = soundSettings.AudioMixerGroup;
        }

        private void GetSoundPossibility(SoundSettings soundSettings)
        {
            var soundPossibilitys = soundSettings.AudioClips;
            var possibility = UnityEngine.Random.Range(1, 101);
            var range = 0;

            for (int i = 0; i < soundPossibilitys.Count; i++)
            {
                var currentPossibility = soundPossibilitys[i];
                range += currentPossibility.Possibility;

                if (possibility <= range)
                {
                    AudioSource.clip = currentPossibility.AudioClip;
                    break;
                }
            }
        }

        private float Volume
        {
            set
            {
                var volume = value * AudioManager.GetVolumeByType(SoundSettings.AudioType);
                AudioSource.volume = volume;
            }
        }

        public bool Pause
        {
            set
            {
                if (_isPaused != value)
                {
                    _isPaused = value;

                    if (_isPaused)
                        AudioSource.Pause();
                    else
                        AudioSource.UnPause();
                }
            }
        }

        ////Parameters: elapsed time, begin value, end value, duration
        private float CubicInOut(float t, float b, float c, float d)
        {
            t /= d / 2;
            if (t < 1)
            {
                return c / 2 * t * t * t + b;
            }

            t -= 2;
            return c / 2 * (t * t * t + 2) + b;
        }
    }
}
