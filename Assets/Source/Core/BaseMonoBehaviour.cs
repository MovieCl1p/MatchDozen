using System;
using UnityEngine;

namespace Core
{
#if UNITY_EDITOR	
	public sealed class ShowDestroyAttribute:Attribute
	{
		
	}
#endif		
	public abstract class BaseMonoBehaviour : MonoBehaviour
	{
		private Transform _transform;
		private bool _released;
		
		public Transform CachedTransform
		{
			get
			{
				if(_transform == null)
					_transform = this.transform;
				
				return _transform;
			}
		}

        protected virtual void Start()
		{
		}

		private void OnApplicationQuit()
		{
			_released = true;
		}

		protected virtual void OnDestroy()
		{
#if UNITY_EDITOR			
			if(this.GetType().GetCustomAttributes(typeof(ShowDestroyAttribute),true).Length == 1)
			{
				MonoLog.Log(MonoLogChannel.Leaks,string.Format("{0} has been destroyed", this.GetType().FullName));
			}			
#endif			
			if(!_released)
			{
				OnReleaseResources();
			}
			
			_released = true;
		}
		
		protected virtual void OnReleaseResources()
		{
		}
    }
}

