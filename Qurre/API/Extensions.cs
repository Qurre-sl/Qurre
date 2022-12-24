using System;
using System.Collections.Generic;
namespace Qurre.API
{
	static public class Extensions
	{
		static public bool TryFind<TSource>(this IEnumerable<TSource> source, out TSource found, Func<TSource, bool> predicate)
		{
			foreach (TSource t in source)
			{
				if (predicate(t))
				{
					found = t;
					return true;
				}
			}
			found = default;
			return false;
		}
	}
}