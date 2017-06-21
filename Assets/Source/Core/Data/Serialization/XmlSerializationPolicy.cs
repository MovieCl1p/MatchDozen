using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Data.Serialization
{
	public sealed class XmlSerializationPolicy : SerializationPolicy
	{
		public XmlSerializationPolicy()
		{
		}
		
		public override string FileExtention
		{
			get 
			{
				return "xml";
			}
		}
		
		public override T Restore<T> (Stream stream)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
			
			return (T)xmlSerializer.Deserialize(stream);
		}
		
		public override void Store<T> (T item,Stream stream)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
			
			xmlSerializer.Serialize(stream,item);
		}
		
		public override T RestoreFromString<T>(string objectData)
		{
		    XmlSerializer serializer = new XmlSerializer(typeof(T));
		    T result;
		
		    using (TextReader reader = new StringReader(objectData))
		    {
		        result = (T)serializer.Deserialize(reader);
		    }
		
		    return result;
		}
		
		public override string StoreToString<T>(T item)
		{
		    XmlSerializer serializer = new XmlSerializer(typeof(T));
		    StringBuilder stringBuilder = new StringBuilder();
		
		    using (TextWriter writer = new StringWriter(stringBuilder))
		    {
		        serializer.Serialize(writer, item);
		    }
		
		    return stringBuilder.ToString();
		}
	}
}

