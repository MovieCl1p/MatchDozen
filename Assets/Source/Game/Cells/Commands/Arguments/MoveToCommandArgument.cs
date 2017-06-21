using UnityEngine;

namespace Game.Cells.Commands.Arguments
{
    public class MoveToCommandArgument
    {
        public readonly Transform Target;
        public readonly Vector3 Destination;
        public readonly float Speed;

        public MoveToCommandArgument(Transform target, Vector3 destination, float speed)
        {
            this.Target = target;
            this.Destination = destination;
            this.Speed = speed;
        }
    }
}
