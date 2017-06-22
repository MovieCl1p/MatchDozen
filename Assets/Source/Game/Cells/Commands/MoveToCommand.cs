using Core;
using Game.Cells.Commands.Arguments;
using UnityEngine;

namespace Cowabunga.UI.Commands
{
	public sealed class MoveToCommand : StateCommand
	{
		private MoveToCommandArgument _parameters; 
		
		protected override void OnStart (object[] args)
		{
			_parameters = (MoveToCommandArgument)args[0];
		}
		
		protected override void OnUpdate ()
		{
		    if (_parameters.Target != null)
		    {
		        Vector2 pos = Vector2.MoveTowards(_parameters.Target.localPosition, _parameters.Destination, _parameters.Speed * Time.deltaTime);
		        _parameters.Target.localPosition = new Vector3(pos.x, pos.y, _parameters.Target.localPosition.z);

		        if (Vector2.Distance(_parameters.Target.localPosition, _parameters.Destination) < 0.01f)
		        {
                    _parameters.Target.localPosition = _parameters.Destination;
                    FinishCommand();
		        }
		    }
		    else
		    {
		        FinishCommand();
		    }
		}
	}
}

