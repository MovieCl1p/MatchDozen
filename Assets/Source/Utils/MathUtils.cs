using System;

namespace Utils
{
	public static class MathUtils
	{
		static public int GetSign(this int value)
		{
			return (value >= 0)? 1 : -1;
		}

		static public int GetSign(this float value)
		{
			return (value >= 0)? 1 : -1;
		}
	}
}