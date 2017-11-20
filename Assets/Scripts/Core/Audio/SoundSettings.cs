
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Core.Audio
{
    public class SoundSettings : ScriptableObject
    {
        public AudioClip AudioClip;
        public List<SoundPossibility> AudioClips;
        [Space]
        public AudioType AudioType;
        public AudioMixerGroup AudioMixerGroup;

        public int Priority;

        public bool Loop;
        public bool Interrupted = true;
        public float FadeIn;
        public float FadeOut;
        public float Timeout;
        [Space]
        public float MinPitch;
        public float MaxPitch;
        [Space]
        public float MinVolume = 1;
        public float MaxVolume = 1;

        public float MinSecondsDiff = 0.01f;
    }

    [Serializable]
    public class SoundPossibility
    {
        public AudioClip AudioClip;
        public int Possibility;
    }
}
