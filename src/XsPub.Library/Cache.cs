using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace XsPub.Library
{
    public class Cache<T> : IEquatable<Cache<T>>  
        where T : class 
    {
        private Lazy<T> _value;
        private readonly Func<T> _valueSelector;

        public Cache(Func<T> valueSelector, LazyThreadSafetyMode mode = LazyThreadSafetyMode.ExecutionAndPublication)
        {
            _valueSelector = valueSelector;
            Mode = mode;
            buildValue(Mode);
        }

        public LazyThreadSafetyMode Mode { get; private set; }
        public bool IsValueCreated { get { return _value.IsValueCreated; } }
        public T Value { get { return _value.Value; } }

        public void Reset()
        {
            buildValue(Mode);
        }

        public override string ToString()
        {
            return _value.Value.ToString();
        }

        public override int GetHashCode()
        {
            return _value.Value.GetHashCode();
        }

        public bool Equals(Cache<T>? other)
        {
            return other is not null && _value.Equals(other._value);
        }

        public override bool Equals(object? other)
        {
            return Equals(other as Cache<T>);
        }

        private void buildValue(LazyThreadSafetyMode mode)
        {
            _value = new Lazy<T>(_valueSelector, mode);
        }
    }
}
