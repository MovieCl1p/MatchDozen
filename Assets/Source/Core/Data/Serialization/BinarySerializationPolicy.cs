using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Data.Serialization
{
	public sealed class BinarySerializationPolicy:SerializationPolicy
	{
		public override string FileExtention
		{
			get 
			{
				return ".bin";
			}
		}
		
		public BinarySerializationPolicy ()
		{
		}
		
		public override T Restore<T> (System.IO.Stream stream)
		{
        	BinaryFormatter formatter = new BinaryFormatter();
			
			return (T)formatter.Deserialize(stream);
			
		}
		
		public override void Store<T> (T item, Stream stream)
		{
        	BinaryFormatter formatter = new BinaryFormatter();
			
			formatter.Serialize(stream,item);
		}
		
		public override T RestoreFromString<T>(string objectData)
	    {
	        byte[] buffer = Convert.FromBase64String(objectData);
	        using (MemoryStream stream = new MemoryStream(buffer))
	        {
	            BinaryFormatter formatter = new BinaryFormatter();
	            stream.Seek(0, SeekOrigin.Begin);
	            return (T)formatter.Deserialize(stream);
	        }
	    }
	
	    public override string StoreToString<T>(T item)
	    {
	        using (MemoryStream stream = new MemoryStream())
	        {
	            BinaryFormatter formatter = new BinaryFormatter();
	            formatter.Serialize(stream, item);
	            stream.Flush();
	            stream.Position = 0;
	            return Convert.ToBase64String(stream.ToArray());
	        }
	    }
	}
}

