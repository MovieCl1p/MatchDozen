
using Assets.Scripts.Game.Model;
using System;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public static class FieldUtils
    {
        //public static Vector3 GetPosition(Vector3 position)
        //{
        //    float dx = (position.x * 100);
        //    float dy = (position.y * 100);
        //    Vector3 newPosition = new Vector3(dx, dy, position.z);

        //    return newPosition;
        //}

        //public static Vector3 GetPosition(Vector2 position)
        //{
        //    float dx = (position.x * 100);
        //    float dy = (position.y * 100);
        //    Vector3 newPosition = new Vector3(dx, dy, 0);

        //    return newPosition;
        //}

        private static Vector2 _pivot;

        public static void SetPivot(Vector2 pivot)
        {
            _pivot = pivot;
        }

        public static Vector3 GetWorldPosition(int x, int y)
        {
            float dx = _pivot.x + (100 * x);
            float dy = _pivot.y + (100 * y);
            
            Vector3 newPosition = new Vector3(dx, dy, 0);

            return newPosition;
        }
        
    }
}