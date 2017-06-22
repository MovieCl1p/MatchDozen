using System;

namespace Utils
{
	public static class EnumUtils
	{
		static public T GetRandomValue<T>(this Enum enums)
		{
			return (T)Enum.GetValues(enums.GetType()).GetRandomValue<T>();
		}
	}
}