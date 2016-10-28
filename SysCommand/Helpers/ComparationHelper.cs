using System.Collections.Generic;
using System.Linq;
using System;

namespace SysCommand.Helpers
{
    public static class ComparationHelper
    {
        public static bool In<TItem>(this TItem source, Func<TItem, TItem, bool> comparer, IEnumerable<TItem> items)
        {
            return items.Any(item => comparer(source, item));
        }

        public static bool In<TItem, T>(this TItem source, Func<TItem, T> selector, IEnumerable<TItem> items)
        {
            return items.Select(selector).Contains(selector(source));
        }

        public static bool In<T>(this T source, IEnumerable<T> items)
        {
            return items.Contains(source);
        }

        public static bool In<TItem>(this TItem source, Func<TItem, TItem, bool> comparer, params TItem[] items)
        {
            return source.In(comparer, (IEnumerable<TItem>)items);
        }

        public static bool In<TItem, T>(this TItem source, Func<TItem, T> selector, params TItem[] items)
        {
            return source.In(selector, (IEnumerable<TItem>)items);
        }

        public static bool In<T>(this T source, params T[] items)
        {
            return source.In((IEnumerable<T>)items);
        }

    }
}
