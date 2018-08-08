using System;
using System.Collections.Generic;

namespace jQueryBuddy.Utilities
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Apply<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var e in source)
            {
                action(e);
                yield return e;
            }
        }
    }
}