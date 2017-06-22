using System;
using System.Runtime.Serialization;
using System.Reflection;
using UnityEngine;


namespace Data.Serialization
{
	public sealed class SerializableField:Attribute
	{
		public readonly int Index;

		public SerializableField(int index)
		{
			Index = index;
		}

		public SerializableField(){}
	}

	[Serializable]
	public abstract class BinarySerializableObject:ISerializable
	{
		#region ISerializable implementation

		public void GetObjectData (SerializationInfo info, StreamingContext context)
		{
			foreach(FieldInfo property in this.GetType().GetFields())
			{
				foreach(SerializableField attribute in 
				        property.GetCustomAttributes(typeof(SerializableField),true))
				{

					if(property.FieldType.IsEnum)
					{
						info.AddValue(attribute.Index.ToString(), property.GetValue(this).ToString());
					}
					else
						info.AddValue(attribute.Index.ToString(),  property.GetValue(this));
				}
			}
		}

		#endregion

		public BinarySerializableObject ()
		{
		}

		public BinarySerializableObject (SerializationInfo info, StreamingContext context)
		{
			foreach(FieldInfo property in this.GetType().GetFields())
			{
				foreach(SerializableField attribute in 
				        property.GetCustomAttributes(typeof(SerializableField),true))
				{
					if(property.FieldType.IsEnum)
					{
						property.SetValue(this, Enum.Parse(property.FieldType,info.GetString(attribute.Index.ToString())));
					}
					else
						property.SetValue(this, info.GetValue(attribute.Index.ToString(),property.FieldType));
				}
			}
		}
	}
}

