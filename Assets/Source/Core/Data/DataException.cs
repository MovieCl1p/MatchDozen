using System;

namespace Data
{
	public sealed class DataException:Exception
	{
		public DataException (String message):base(message)
		{
		}
		
		public DataException (String message, Exception innerException):base(message,innerException)
		{
		}
	}
}

