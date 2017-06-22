using UnityEngine;

namespace Core
{
	public abstract class MonoScheduledBehaviour : BaseMonoBehaviour
	{
		private float _scheduleUpdateInterval;
		private float _scheduleUpdateTime;
		
		[SerializeField]
		private bool _scheduledUpdate;
		private bool _scheduleRepeat;	
		
		protected void UnscheduleUpdate()
		{
			_scheduledUpdate = false;			
		}
		
		protected void ScheduleUpdate(float interval, bool repeat = true)
		{
			_scheduledUpdate = true;
			_scheduleUpdateInterval = interval;
			_scheduleRepeat = repeat;
			_scheduleUpdateTime = Time.time + _scheduleUpdateInterval;
		}
		
		protected virtual void Update()
		{			
			if(_scheduledUpdate && Time.time > _scheduleUpdateTime)
			{
				if(_scheduleRepeat)
					_scheduleUpdateTime = Time.time + _scheduleUpdateInterval;
				else
					_scheduledUpdate = false;
				
				OnScheduledUpdate();						
			}	
		}
		
		protected virtual void OnScheduledUpdate()
		{
			
		}	
	}
}

