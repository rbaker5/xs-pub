using System;
using System.Threading;

namespace XsPub.Library.Utility
{
    internal static class Lazy
    {
        internal static Lazy<T> Create<T>(Func<T> valueSelector) where T : class
        {
            return new Lazy<T>(valueSelector);
        }

        internal static Lazy<T> Create<T>(Func<T> valueSelector, LazyThreadSafetyMode mode) where T : class
        {
            return new Lazy<T>(valueSelector, mode);
        }
    }
}