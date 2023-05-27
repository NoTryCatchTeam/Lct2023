using System;
using System.Collections.Generic;

namespace Lct2023.Commons.Extensions
{
    public static class EnumerableExtensions
    {
        public static void ForEach<TItem>(this IEnumerable<TItem> items, Action<TItem> action)
        {
            if (items == null)
            {
                return;
            }

            foreach (var item in items)
            {
                action(item);
            }
        }
    }
}