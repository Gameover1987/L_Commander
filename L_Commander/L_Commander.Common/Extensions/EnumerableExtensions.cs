using System;
using System.Collections.Generic;
using System.Linq;

namespace L_Commander.Common.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable)
            {
                action(item);
            }

            return enumerable;
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
                return true;

            return !enumerable.Any();
        }

        public static bool IntersectsWith<T>(this IEnumerable<T> enumerableA, IEnumerable<T> enumerableB)
        {
            return enumerableA.Any(enumerableB.Contains);
        }
    }
}
