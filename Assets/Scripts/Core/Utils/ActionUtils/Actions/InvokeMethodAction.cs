using System;

namespace Core.Utils.ActionUtils.Actions
{
    public class InvokeMethodAction : BaseAction
    {
        private readonly Action _method;

        public InvokeMethodAction(Action method)
        {
            _method = method;
        }

        public override void Execute()
        {
            _method.Invoke();
            Complete();
        }
    }
}
