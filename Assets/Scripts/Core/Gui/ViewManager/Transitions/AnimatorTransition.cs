using System;
using System.Collections;
using UnityEngine;

namespace Core.Gui.ViewManager.Transitions
{
    public class AnimatorTransition : AbstractTransition
    {
        [HideInInspector]
        public Animator Animator;

        [HideInInspector]
        public string TriggerName;

        [HideInInspector]
        public string ClipName;

        [HideInInspector]
        public int SelectedClip;

        [HideInInspector]
        public int SelectedTrigger;

        public override Action OnComplete { get; set; }

        private Coroutine _currentCoroutine;

        private AnimationClip GetClip()
        {
            AnimationClip[] clips = Animator.runtimeAnimatorController.animationClips;

            foreach (AnimationClip clip in clips)
            {
                if (clip.name == ClipName)
                {
                    return clip;
                }
            }

            return null;
        }

        public override AbstractTransition Play()
        {
            var currentClip = GetClip();

            Animator.Update(0);

            if (!string.IsNullOrEmpty(TriggerName) && TriggerName != "None")
            {
                Animator.SetTrigger(TriggerName);
            }

            if (currentClip != null)
            {
                _currentCoroutine = StartCoroutine(OnAnimationComplete(currentClip.length));
            }

            return this;
        }

        public override AbstractTransition Play(string clipName, string triggerName)
        {
            TriggerName = triggerName;
            ClipName = clipName;
            return Play();
        }

        public override void Activate(bool state)
        {
            if (Animator != null)
            {
                Animator.SetTrigger(state ? "Deactivate" : "Activate");
            }
        }

        private IEnumerator OnAnimationComplete(float time)
        {
            yield return new WaitForSecondsRealtime(time);
            yield return null;

            if (OnComplete != null)
                OnComplete.Invoke();

            OnComplete = null;
        }

        private void OnDestroy()
        {
            if (_currentCoroutine != null)
                StopCoroutine(_currentCoroutine);
        }
    }
}
