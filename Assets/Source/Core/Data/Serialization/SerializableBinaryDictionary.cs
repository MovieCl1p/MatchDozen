using System;
using System.Runtime.Serialization;
using System.Collections.Generic;


namespace Data.Serialization
{
	[Serializable]
	public sealed class SerializableBinaryDictionary<V>:Dictionary<string,V>,ISerializable
	{
		private const string KEYS = "___keys";

		#region ISerializable implementation

		public new void GetObjectData (SerializationInfo info, StreamingContext context)
		{
			List<string> keys = new List<string>();

			foreach(string key in this.Keys)
			{
				keys.Add(key);

				info.AddValue(key,this[key]);
			}

			info.AddValue(KEYS, keys.ToArray());
		}

		public SerializableBinaryDictionary(SerializationInfo info, StreamingContext context)
		{
			string[] keys = (string[])info.GetValue(KEYS,typeof(string[]));

			foreach(string key in keys)
			{
				this.Add(key,(V)info.GetValue(key,typeof(V)));
			}
		}

		#endregion

		public SerializableBinaryDictionary ()
		{
		}
	}
}

