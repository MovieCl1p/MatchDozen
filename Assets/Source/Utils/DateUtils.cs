using System;
namespace Utils
{
	public static class DateUtils
	{
		static public string ToStringTime(this int value)
		{
			return ToStringTime((float)value);
		}

		static public string ToStringTime(this float value)
		{
			return new DateTime(TimeSpan.FromSeconds(value).Ticks).ToString("mm:ss");
		}
	}
}