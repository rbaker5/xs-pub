using System;
using System.Collections.Generic;
using System.Threading;

namespace XsPub.Library
{
    // A resettable lazy wrapper: evaluates _valueSelector on first access,
    // then caches the result. Reset() discards the cached result so the next
    // access re-evaluates.
    //
    // Equality is value-based: a Cache<T>, a Lazy<T>, and a raw T are all
    // equal when they hold the same value. Note: GetHashCode() returns the
    // value's hash code, so GetHashCode() is consistent across Cache<T>
    // instances but NOT between Cache<T> and Lazy<T> (Lazy<T> uses identity
    // hash and cannot be changed).
    public class Cache<T> : IEquatable<Cache<T>>, IEquatable<Lazy<T>>, IEquatable<T>
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
        public bool IsValueCreated => _value.IsValueCreated;
        public T Value => _value.Value;

        public void Reset() => buildValue(Mode);

        public override string ToString() => Value.ToString()!;

        public override int GetHashCode() => EqualityComparer<T>.Default.GetHashCode(Value!);

        public bool Equals(Cache<T>? other)
            => other is not null && EqualityComparer<T>.Default.Equals(Value, other.Value);

        public bool Equals(Lazy<T>? other)
            => other is not null && EqualityComparer<T>.Default.Equals(Value, other.Value);

        public bool Equals(T? other)
            => EqualityComparer<T>.Default.Equals(Value, other);

        public override bool Equals(object? other) => other switch
        {
            Cache<T> cache => Equals(cache),
            Lazy<T>  lazy  => Equals(lazy),
            T        value => Equals(value),
            _              => false
        };

        private void buildValue(LazyThreadSafetyMode mode)
            => _value = new Lazy<T>(_valueSelector, mode);
    }
}
