
using System;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class MoveToCommand : MonoBehaviour
    {
        private Vector3 _destination;
        private float _speed;

        protected void Update()
        {
            Vector2 pos = Vector2.MoveTowards(transform.localPosition, _destination, _speed * Time.deltaTime);
            transform.localPosition = new Vector3(pos.x, pos.y, transform.localPosition.z);

            if (Vector2.Distance(transform.localPosition,_destination) < 0.01f)
            {
                transform.localPosition = _destination;
                Destroy(this);
            }
        }

        public void Init(Vector3 viewPosition, float moveSpeed)
        {
            _destination = viewPosition;
            _speed = moveSpeed;
        }
    }
}