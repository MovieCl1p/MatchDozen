using System;
using System.Collections;

namespace Utils
{
	public static class ArrayUtils
	{
		private static Random random = new Random();

		static public Array Shuffle(this Array array)
		{
			for (int n = array.Length - 1; n > 0; --n)
			{
				int k = random.Next(n+1);
				object temp = array.GetValue(n);
				array.SetValue(array.GetValue(k), n);
				array.SetValue(temp, k);
			}
			return array;
		}

		static public T GetRandomValue<T>(this Array array)
		{
			return (T)array.GetValue(UnityEngine.Random.Range(0, array.Length-1));
		}
	}
}