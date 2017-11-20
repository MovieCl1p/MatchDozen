using Core.Audio;
using Core.Gui.ViewManager.Transitions;
using strange.extensions.signal.impl;
using System;
using UnityEngine;

namespace Core.Gui.ViewManager
{
    public class ViewBase : strange.extensions.mediation.impl.View
    {
        public Signal TransitionInCompleteSignal { get; private set; }
        public Signal TransitionOutCompleteSignal { get; private set; }
        public Signal StartTransitionInSignal { get; private set; }
        public Signal StartTransitionOutSignal { get; private set; }

        public event Action OnCreatedEvent;
        public event Action OnAndroidBackClick;
        public event Action OnActivateView;
        public event Action OnDeactivateView;

        public object Options { get; set; }
        public ViewLayer Layer { get; set; }
        public bool IsDebug { get; set; }

        [SerializeField]
        protected AbstractTransition TransitionIn;

        [SerializeField]
        private AbstractTransition TransitionOut;

        public SoundSettings SoundPlayIn;
        public SoundSettings SoundPlayOut;

        protected bool IsRemoving;
        protected IViewManager ViewManager;
        public bool IsDeactivated { get; set; }
        public bool IsManualActivation { get; set; }

        public ViewBase()
        {
            TransitionInCompleteSignal = new Signal();
            TransitionOutCompleteSignal = new Signal();
            StartTransitionOutSignal = new Signal();
            StartTransitionInSignal = new Signal();
            ViewManager = AppManager.GetInstance().ViewManager;
        }

        protected override void Start()
        {
            base.Start();

            if (OnCreatedEvent != null)
            {
                OnCreatedEvent.Invoke();
                OnCreatedEvent = null;
            }

            PlayIn();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!IsDeactivated)
                {
                    if (OnAndroidBackClick != null)
                        OnAndroidBackClick();
                }
            }
        }

        private void PlayIn()
        {
            if (!IsDeactivated)
            {
                StartTransitionInSignal.Dispatch();

                if (TransitionIn != null)
                {
                    TransitionIn.OnComplete += OnPlayInComplete;
                    TransitionIn.Play();
                }
                else
                {
                    OnPlayInComplete();
                }

                if (SoundPlayIn != null)
                {
                    AudioManager.Play(SoundPlayIn);
                }
            }
            else
            {
                OnPlayInComplete();
            }
        }

        private void OnPlayInComplete()
        {
            TransitionInCompleteSignal.Dispatch();
        }

        private void PlayOut()
        {
            StartTransitionOutSignal.Dispatch();

            if (TransitionOut != null && !IsDeactivated)
            {
                TransitionOut.OnComplete += OnPlayOutComplete;
                TransitionOut.Play();
            }
            else
            {
                OnPlayOutComplete();
            }

            if (SoundPlayOut != null)
            {
                AudioManager.Play(SoundPlayOut);
            }
        }

        private void OnPlayOutComplete()
        {
            AppManager.GetInstance().ViewManager.RemoveView(this);
            TransitionOutCompleteSignal.Dispatch();
        }

        public ViewBase ChangeView(string viewId, object options = null)
        {
            return ViewManager.SetView(viewId, options);
        }

        public void OpenPreviouseView()
        {
            ViewManager.OpenPreviouseView(this);
        }

        public void Activate()
        {
            if (IsRemoving || !IsDeactivated)
                return;

            IsManualActivation = false;
            ChangeActiveState(false);

            if (OnActivateView != null)
                OnActivateView();
        }

        public void DeActivate()
        {
            if (IsRemoving || IsDeactivated)
                return;

            ChangeActiveState(true);

            if (OnDeactivateView != null)
                OnDeactivateView();
        }

        protected virtual void ChangeActiveState(bool isDeactivated)
        {
            IsDeactivated = isDeactivated;

            if (TransitionIn != null)
                TransitionIn.Activate(isDeactivated);
        }

        public void CloseView()
        {
            if (IsRemoving)
                return;

            IsRemoving = true;
            PlayOut();
        }
    }
}
