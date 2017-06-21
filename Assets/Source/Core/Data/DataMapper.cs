using System;
using System.IO;
using Data.Serialization;
using Domain;

namespace Data
{
	public abstract class DataMapper<TDomainObject,TSerializationPolicy> 
		where TDomainObject:DomainObject
		where TSerializationPolicy:SerializationPolicy,new()
	{
		private TSerializationPolicy _seializer;
		
		protected SerializationPolicy Serializer
		{
			get
			{
				if(_seializer == null)
					_seializer = new TSerializationPolicy();
				
				return _seializer;
			}
		}
		
		protected abstract string BuildFolderPath(string objectId);
				
		protected virtual String BuildPath(string objectId)
		{
			return Path.Combine(BuildFolderPath(objectId),Path.ChangeExtension(objectId,Serializer.FileExtention));
		}
		
		public virtual void Save(TDomainObject item)
		{
			string folderPath = BuildFolderPath(item.Id);
			
			if(!Directory.Exists(folderPath))
				Directory.CreateDirectory(folderPath);
						
			using(Stream stream = File.Open(BuildPath(item.Id),FileMode.Create,FileAccess.Write))
			{
				Serializer.Store<TDomainObject>(item,stream);
			}
		}
		
		public virtual TDomainObject LoadById(String id)
		{
			using(Stream stream = File.OpenRead(BuildPath(id)))
			{
				return Serializer.Restore<TDomainObject>(stream);
			}
		}
	}
}

