using System;
using System.Collections.Generic;
using Game.UI.PopUp;
using UI;
using UnityEngine;

namespace RoomRumble.UI
{
	public sealed class PopUpFactory:IPopUpFactory
	{
		private static Dictionary<Type,string> _typeToPrefab;
		
		static PopUpFactory ()
		{
			_typeToPrefab = new Dictionary<Type, string>();
			
			_typeToPrefab.Add(typeof(FinishPopupController), "Prefabs/UI/PopUp/FinishPopup");
	
		}
		
		#region IPopUpFactory implementation
		public T CreateInstance<T> (UIController parentController) where T : UIPopUpController
		{
			if(!_typeToPrefab.ContainsKey(typeof(T)))
				throw new Exception(string.Format( "PopUp Type {0} does not have mapped prefab", typeof(T)));
			
			GameObject gameObject = (GameObject)GameObject.Instantiate(Resources.Load(_typeToPrefab[typeof(T)]));
			
			T controller = gameObject.GetComponent<T>();
			
			return controller;
		}
		#endregion
	}
}

