using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XsPub.Library
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<TResult> EmptyIfNull<TResult>(this IEnumerable<TResult> source)
        {
            return source ?? Enumerable.Empty<TResult>();
        }

        public static ILookup<TKey, TValue> LookupFromItem1<TKey, TValue>(this IEnumerable<Tuple<TValue, TKey>> source)
        {
            ArgumentNullException.ThrowIfNull(source);

            return source.ToLookup(pair => pair.Item2, pair => pair.Item1);
        }

        public static ILookup<TKey, TValue> LookupFromItem2<TKey, TValue>(this IEnumerable<Tuple<TKey, TValue>> source)
        {
            ArgumentNullException.ThrowIfNull(source);

            return source.ToLookup(pair => pair.Item1, pair => pair.Item2);
        }

        public static IDictionary<TKey, TValue> DictionaryFromItem1<TKey, TValue>(this IEnumerable<Tuple<TValue, TKey>> source)
        {
            ArgumentNullException.ThrowIfNull(source);

            return source.ToDictionary(pair => pair.Item2, pair => pair.Item1);
        }

        public static IDictionary<TKey, TValue> DictionaryFromItem2<TKey, TValue>(this IEnumerable<Tuple<TKey, TValue>> source)
        {
            ArgumentNullException.ThrowIfNull(source);

            return source.ToDictionary(pair => pair.Item1, pair => pair.Item2);
        }
    }

    internal static class DictionaryExtensions
    {
        public static IDictionary<TKey, TValue> InvertDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TValue, TKey>> source)
        {
            ArgumentNullException.ThrowIfNull(source);

            return source.ToDictionary(pair => pair.Value, pair => pair.Key);
        }

        public static ILookup<TKey, TValue> InvertToLookup<TKey, TValue>(this IEnumerable<KeyValuePair<TValue, TKey>> source)
        {
            ArgumentNullException.ThrowIfNull(source);

            return source.ToLookup(pair => pair.Value, pair => pair.Key);
        }
    }
}
