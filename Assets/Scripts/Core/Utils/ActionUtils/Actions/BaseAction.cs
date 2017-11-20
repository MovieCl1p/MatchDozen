using System;

namespace Core.Utils.ActionUtils.Actions
{
    public class BaseAction : IAction
    {
        public event Action OnComplete;
        public event Action<string> OnFail;

        public virtual void Execute()
        {

        }

        protected void Complete()
        {
            if (OnComplete != null)
                OnComplete.Invoke();
        }

        protected void Fail(string error)
        {
            if (OnFail != null)
                OnFail.Invoke(error);
        }

        public virtual void Destroy()
        {

        }
    }
}
