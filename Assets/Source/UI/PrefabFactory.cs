using System;
using System.Collections.Generic;
using UnityEngine;


namespace RoomRumble.UI
{
	public enum PrefabName
	{
		DepthCamera
	}

	public static class PrefabFactory
	{
		private static Dictionary<Type,String> _typeToPrefabMapping;
		private static Dictionary<PrefabName,String> _prefabTypeToPrefabMapping;
		
		static PrefabFactory ()
		{
			_typeToPrefabMapping = new Dictionary<Type, string>();

//			_typeToPrefabMapping.Add(typeof(FurnitureController),"Prefabs/Element");
//			_typeToPrefabMapping.Add(typeof(LevelController),"Prefabs/LevelController");

			_prefabTypeToPrefabMapping = new Dictionary<PrefabName, string>();

			_prefabTypeToPrefabMapping.Add(PrefabName.DepthCamera,"Prefabs/Camera/DepthCamera");

			
//			_typeToPrefabMapping.Add(typeof(BottomGameMenuButton),"Prefabs/GameMenu/gameMenuButton");


		}
		
		public static T CreateInstance<T>()
			where T:MonoBehaviour
		{
			String path = _typeToPrefabMapping[typeof(T)];
			
			GameObject gameObject = (GameObject)GameObject.Instantiate(Resources.Load(path));
						
			gameObject.name = typeof(T).Name;
			
			return gameObject.GetComponent<T>();
		}
		
		public static T CreateInstance<T>(GameObject prefab)
			where T:MonoBehaviour
		{
			GameObject gameObject = (GameObject)GameObject.Instantiate(prefab);
						
			gameObject.name = typeof(T).Name;
			
			return gameObject.GetComponent<T>();
		}

		public static GameObject CreateInstance(Type type)
		{
			String path = _typeToPrefabMapping[type];
			
			GameObject gameObject = (GameObject)GameObject.Instantiate(Resources.Load(path));
						
			gameObject.name = type.Name;
			
			return gameObject;
		}

		public static GameObject CreateInstance(PrefabName prefabName)
		{
			String path = _prefabTypeToPrefabMapping[prefabName];
			
			GameObject gameObject = (GameObject)GameObject.Instantiate(Resources.Load(path));

			gameObject.name = prefabName.ToString();
			
			return gameObject;
		}
	}
}

