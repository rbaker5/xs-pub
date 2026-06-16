using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XsPub.Library
{
    public static class ObjectExtensions
    {
        public static TResult IfNotNull<TInput, TResult>(this TInput input, Func<TInput, TResult> actionIfNull, TResult defaultValue)
            where TInput : class
        {
            return input == null ? defaultValue : actionIfNull(input);
        }

        public static TResult IfNotNull<TInput, TResult>(this TInput input, Func<TInput, TResult> actionIfNull)
            where TInput : class
        {
            return input == null ? default(TResult) : actionIfNull(input);
        }
    }
}
