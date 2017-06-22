using System;
using System.Collections.Generic;

namespace Core
{
	public abstract class IdentityMap<T>
	{
		private readonly Dictionary<String,T> _items;
		
		public IdentityMap ()
		{
			_items = new Dictionary<string, T>();
		}
		
		protected abstract String MakeKey(T item);
		protected abstract void Merge(T currentItem, T updatedItem);
		
		public T Add(T item)
		{
			String key = MakeKey(item);
			
			
			if(_items.ContainsKey(key))
				Merge(_items[key],item);
			else
				_items.Add(key,item);
			
			
			return _items[key];
		}
		
		
		public T Get(string key)
		{
			if(!_items.ContainsKey(key))
			{
				MonoLog.LogWarning(MonoLogChannel.Core, string.Format("Object with key {0} does not exists", key));
			}
			
			return _items[key];
		}
		
		public bool Contains(string key)
		{
			return _items.ContainsKey(key);
		}
	}
}