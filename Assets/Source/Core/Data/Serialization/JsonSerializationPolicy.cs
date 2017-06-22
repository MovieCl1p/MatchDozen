using System;
using System.IO;
using Newtonsoft.Json;

namespace Data.Serialization
{

	public sealed class JsonSerializationPolicy : SerializationPolicy
	{
		public JsonSerializationPolicy ()
		{
		}
		
		public override T Restore<T> (System.IO.Stream stream)
		{
			using(StreamReader streamReader = new StreamReader(stream))
			{
				return JsonConvert.DeserializeObject<T>(streamReader.ReadToEnd());
			}
		}

	    public override string StoreToString<T>(T item)
	    {
	        throw new NotImplementedException();
	    }

	    public override T RestoreFromString<T>(string objectData)
	    {
	        throw new NotImplementedException();
	    }

	    public override void Store<T> (T item, Stream stream)
		{
			using(TextWriter textWriter = new StreamWriter(stream))
			{
				textWriter.Write(JsonConvert.SerializeObject(item));
			}
		}
		
		public override string FileExtention 
		{
			get 
			{
				return ".json";
			}
		}
	}
}

