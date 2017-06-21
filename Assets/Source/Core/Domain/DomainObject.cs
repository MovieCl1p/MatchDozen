using System;
using Core;
using System.Xml.Serialization;

namespace Domain
{
	public abstract class DomainObject:Observable
	{
		[XmlAttribute]
		public string Id;
		
		public DomainObject ()
		{
			this.Id = Guid.NewGuid().ToString();			
		}
	}
}

