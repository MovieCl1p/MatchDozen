using System;

namespace Core
{
	public class Responder<T>
	{
		public readonly Action<T> result;
		public readonly Action<T> fault;
		
		public Responder(Action<T> result, Action<T> fault)
		{
			this.result = result;
			this.fault = fault;
		}
		
		public Responder(Action<T> result)
		{
			this.result = result;
		}
	}
	
}

