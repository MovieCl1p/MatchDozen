using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Data.Serialization;

namespace Core
{
	
	public interface IObservable
	{
		void SetChanged();
		void AddObserver(Observer observer);
		void RemoveObserver(Observer observer);
	}
	
	[Serializable]
	public abstract class Observable:BinarySerializableObject, IObservable
	{
		[NonSerialized]
		private List<Observer> _observers;
		
		[XmlIgnore]
		public bool IsChanged
		{
			get;
			private set;
		}

		public void SetChanged()
		{
			IsChanged = true;
			
			if(_observers != null)
			{
				for(int i = _observers.Count-1; i >=0;i--)
					_observers[i].OnObjectChanged(this);
			}
		}
		
		public void Commit()
		{
			IsChanged = false;
		}
		
		public void SetChangedAndCommit()
		{
			SetChanged();
			
			Commit();
		}
		
		public void AddObserver(Observer observer)
		{
			if(_observers == null)
				_observers = new List<Observer>();
			
			_observers.Add(observer);
		}
		
		public void RemoveObserver(Observer observer)
		{
			if(_observers == null)
				return;
			
			_observers.Remove(observer);
		}

		public void Clear()
		{
			if(_observers != null)
				_observers.Clear();
		}
	}
}

