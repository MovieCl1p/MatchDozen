using System;
using System.IO;

namespace Data.Serialization
{
	public abstract class SerializationPolicy
	{
		public abstract String FileExtention{get;}
		
		public abstract void Store<T>(T item,Stream stream);

		public abstract T Restore<T>(Stream stream);
		
		public abstract string StoreToString<T>(T item);

		public abstract T RestoreFromString<T>(string objectData);
	}
}

