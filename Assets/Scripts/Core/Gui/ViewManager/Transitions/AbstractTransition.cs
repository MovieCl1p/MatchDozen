using System;
using UnityEngine;

namespace Core.Gui.ViewManager.Transitions
{
    public abstract class AbstractTransition : MonoBehaviour
    {
        public abstract Action OnComplete { set; get; }
        public abstract AbstractTransition Play();
        public abstract AbstractTransition Play(string clipName, string triggerName);
        public abstract void Activate(bool state);
    }
}
