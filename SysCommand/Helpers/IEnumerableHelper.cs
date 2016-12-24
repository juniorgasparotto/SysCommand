using System.Collections.Generic;
using System.Linq;
using System;

namespace SysCommand.Helpers
{
    public static class IEnumerableHelper
    {
        public static bool Empty<TSource>(this IEnumerable<TSource> source)
        {
            return !source.Any();
        }

        public static bool Empty<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return !source.Any(predicate);
        }

        public static IEnumerable<T> Replace<T>(this IEnumerable<T> source, T oldValue, T newValue)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            var lst = source.ToList();
            var index = lst.IndexOf(oldValue);
            if (index != -1)
                lst[index] = newValue;
            return lst;
        }
    }
}
