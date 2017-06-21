using System.Collections.Generic;

namespace Core
{
	public sealed class AsyncToken<T>
	{
		private readonly List<Responder<T>> _responders;
		
		private T _command;
		
		
		public AsyncToken (T command)
		{
			_responders = new List<Responder<T>>();
			_command = command;
		}
		
		public void AddResponder(Responder<T> responder)
		{
			_responders.Add(responder);
		}
		
		internal void FireResponse()
		{
			foreach(Responder<T> responder in _responders)
			{
				responder.result(_command);
			}
			
			_responders.Clear();
		}
		
		internal void FireFault()
		{
			foreach(Responder<T> responder in _responders)
			{
				if(responder.fault != null)
					responder.fault(_command);
			}
			
			_responders.Clear();			
		}
	}
}