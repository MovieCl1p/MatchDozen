
using UnityEngine;

namespace Game
{
    public static class FieldUtils
    {

        public static Vector3 GetPosition(Vector3 position)
        {
            float dx = -6.5f + (position.x * 4);
            float dy = -15 + (position.y * 4);
            Vector3 newPosition = new Vector3(dx, dy, position.z);
            
            return newPosition;
        }
    }
}
