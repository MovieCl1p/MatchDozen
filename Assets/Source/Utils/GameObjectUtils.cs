using System;
using UnityEngine;


namespace Utils
{
	public static class GameObjectUtils
	{
		static public void SetLayerRecursevly(this GameObject activeGameObject, int layer)
		{
			activeGameObject.layer = layer;

			foreach (Transform child in activeGameObject.transform)
			{
				child.gameObject.SetLayerRecursevly(layer);
			}
		}
	}
}