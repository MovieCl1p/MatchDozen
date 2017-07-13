
using UnityEngine;

namespace Assets.Scripts.Game
{
    public static class FieldUtils
    {
        public static Vector3 GetPosition(Vector3 position)
        {
            float dx = (position.x * 100);
            float dy = (position.y * 100);
            Vector3 newPosition = new Vector3(dx, dy, position.z);

            return newPosition;
        }

        public static Vector3 GetPosition(Vector2 position)
        {
            float dx = (position.x * 100);
            float dy = (position.y * 100);
            Vector3 newPosition = new Vector3(dx, dy, 0);

            return newPosition;
        }
    }
}