
using Assets.Scripts.Game.Model;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class CellModel
    {
        public GridPoint GridPosition { get; set; }

        public Vector2 WorldsPosition { get; set; }

        public int Count { get; set; }

        public float MoveSpeed { get; set; }

        public CellModel(float speed)
        {
            MoveSpeed = speed;
        }
    }
}