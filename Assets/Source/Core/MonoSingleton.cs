using UnityEngine;

namespace Core
{
	public abstract class MonoSingleton : MonoScheduledBehaviour
	{
		public const string MG_NAME = "Melior Games";
		
		private static GameObject _mgGameObject;
		
		public static GameObject GetMGGameObject()
		{
			if (MonoSingleton._mgGameObject != null)
			{
				return MonoSingleton._mgGameObject;
			}
			
			MonoSingleton._mgGameObject = GameObject.Find(MG_NAME);
			
			if (MonoSingleton._mgGameObject == null)
			{
				MonoSingleton._mgGameObject = new GameObject(MG_NAME);
				UnityEngine.Object.DontDestroyOnLoad(MonoSingleton._mgGameObject);
			}
			return MonoSingleton._mgGameObject;
		}
	}
	
	public abstract class MonoSingleton<T> : MonoSingleton where T : MonoSingleton<T>
	{
	    private static T _instance = null;
	    public static T Instance
	    {
	        get
	        {
				if(_instance == null)
				{
					Initialize();
				}
				
				return _instance;
	        }
	    }
		
		public static void Initialize()
		{
			MonoBehaviour monoBehaviour = UnityEngine.Object.FindObjectOfType(typeof(T)) as MonoBehaviour;
			
			if (monoBehaviour == null)
			{
				GameObject mgGameObject = MonoSingleton.GetMGGameObject();
				
				GameObject gameObject = new GameObject(typeof(T).ToString());
				
				gameObject.AddComponent(typeof(T));
				gameObject.transform.parent = mgGameObject.transform;
				
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
				
				_instance = gameObject.GetComponent<T>();
			}
			else
				_instance = monoBehaviour.GetComponent<T>();
		}
		
	    private void Awake()
	    {
	         Init();
	    }
	 
	    protected virtual void Init(){}
	 
	    private void OnApplicationQuit()
	    {
	        _instance = null;
	    }
	}
	
	/*public abstract class MonoSingletonWithUpdate<T>:MonoSingleton<T> where T:MonoSingletonWithUpdate<T>
	{
		private float _interval;
		float _nextUpdate;
		
		public MonoSingletonWithUpdate(float interval)
		{
			_interval = interval;
		}
			
		private void Update()
		{
			if(	Time.time > _nextUpdate )
			{
				OnUpdate();
				
				_nextUpdate = Time.time + _interval;
			}
		}
		
		protected virtual void OnUpdate()
		{
			
		}
		
		protected void ChangeInterval(float interval)
		{
			_interval = interval;
			_nextUpdate = Time.time + _interval;
		}
	}*/
	
}