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

        /// <summary>
        /// Adds the range to the source ist.
        /// </summary>
        /// <typeparam name="T">The type of the item.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="items">The items.</param>
        /// <exception cref="System.ArgumentNullException">
        /// source
        /// or
        /// items.
        /// </exception>
        public static void AddRange<T>(this IList<T> source, IEnumerable<T> items)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (items is null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            switch (source)
            {
                case List<T> list:
                    list.AddRange(items);
                    break;

                default:
                    items.ForEach(source.Add);
                    break;
            }
        }
    }
}
