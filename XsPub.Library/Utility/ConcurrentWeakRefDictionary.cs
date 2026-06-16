using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using XsPub.Library.Properties;

namespace XsPub.Library.Utility
{
    /// <summary>
    /// Represents a dictionary which stores the values as weak references instead of strong
    /// references. Null values are supported.
    /// </summary>
    /// <typeparam name="TKey">The key type</typeparam>
    /// <typeparam name="TValue">The value type</typeparam>
    public class ConcurrentWeakRefDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly ConcurrentDictionary<TKey, WeakReference> _inner = new ConcurrentDictionary<TKey, WeakReference>();

        /// <summary>
        /// Determines if the dictionary contains a value for the key.
        /// </summary>
        /// <param name="key">The key to look for.</param>
        /// <returns>true if the key is contained in the dictionary; otherwise, false.</returns>
        public bool ContainsKey(TKey key)
        {
            TValue dummy;
            return TryGetValue(key, out dummy);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the dictionary.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerator"/> object that can be used to iterate through the dictionary. 
        /// </returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var kvp in _inner)
            {
                object innerValue = kvp.Value.Target;

                if (innerValue != null)
                    yield return new KeyValuePair<TKey, TValue>(kvp.Key, DecodeNullObject<TValue>(innerValue));
                else
                    ((ICollection<KeyValuePair<TKey, WeakReference>>)_inner).Remove(kvp);
            }
        }

        public void Clear()
        {
            _inner.Clear();
            Contract.Assume(Count == 0);
        }
       
        /// <summary>
        /// Attempts to get a value from the dictionary.
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        /// <returns>Returns true if the value was present; false otherwise.</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            value = default(TValue);
            WeakReference wr;

            if (!_inner.TryGetValue(key, out wr))
            {
                Contract.Assume(!ContainsKey(key));
                return false;
            }

            object result = wr.Target;

            if (result == null)
            {
                // Only remove the item if the weak reference still matches.
                ((ICollection<KeyValuePair<TKey, WeakReference>>)_inner).Remove(new KeyValuePair<TKey, WeakReference>(key, wr));
                // A gray area here, if Remove returned true, then item does not exist.  But if it returned false the item was
                // created after this method started executing.  We could recurse but instead we take the simpler route of 
                // reporting that the item does not exist since no garauntees of the currency of this method are made except that
                // it won't return a reference to an invalid object.
                Contract.Assume(!ContainsKey(key));
                return false;
            }

            value = DecodeNullObject<TValue>(result);
            Contract.Assume(ContainsKey(key));
            return true;
        } 

        public bool TryAdd(TKey key, TValue value)
        {
            TValue dummy;
            if (TryGetValue(key, out dummy))
                return false;

            return _inner.TryAdd(key, new WeakReference(EncodeNullObject(value)));
        }

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            object innerResult = default(TValue);
            var actualReference =
                _inner.AddOrUpdate(key,
                                   innerKey =>
                                       {
                                           // By assigning to variable in outer scope we've created a strong reference.
                                           innerResult = valueFactory(innerKey);
                                           return new WeakReference(EncodeNullObject(innerResult));
                                       },
                                   (innerKey, innerReference) =>
                                   {
                                       // By assigning to variable in outer scope we've created a strong reference.
                                       object referenceValue = innerReference.Target;
                                       innerResult = referenceValue ?? valueFactory(innerKey);
                                       return referenceValue == null ? new WeakReference(EncodeNullObject(innerResult)) : innerReference;
                                   });

            // In case contention somehow caused a different return.  The reference should still be valid if so.
            object outerResult = actualReference.Target;
            innerResult = outerResult;
            return DecodeNullObject<TValue>(innerResult);
        }

        public TValue GetOrAdd(TKey key, TValue value)
        {
            object result = default(TValue);
            // If we create a new reference we're insured there is a strong reference by parameter "value".
            var actualReference =
                _inner.AddOrUpdate(key,
                                   innerKey => new WeakReference(EncodeNullObject(value)),
                                   (innerKey, innerReference) =>
                                       {
                                           // By assigning to variable in outer scope we've created a strong reference that will last long enough for outer method to exit.
                                           result = innerReference.Target;
                                           return result == null ? new WeakReference(EncodeNullObject(value)) : innerReference;
                                       });

            // In case contention somehow caused a different return.  The reference should still be valid if so.
            result = actualReference.Target;
            return DecodeNullObject<TValue>(result);
        }

        public TValue AddOrUpdate(TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
        {
            object result = default(TValue);
            var actualReference =
                _inner.AddOrUpdate(key,
                                   innerKey =>
                                   {
                                       // By assigning to variable in outer scope we've created a strong reference.
                                       result = addValueFactory(innerKey);
                                       return new WeakReference(EncodeNullObject(result));
                                   },
                                   (innerKey, innerReference) =>
                                   {
                                       // By assigning to variable in outer scope we've created a strong reference that will last long enough for outer method to exit.
                                       result = innerReference.Target;
                                       // If the reference is dead, create a new value, else update the existing one.
                                       result = result == null ? addValueFactory(innerKey) : updateValueFactory(innerKey, DecodeNullObject<TValue>(result));
                                       return new WeakReference(EncodeNullObject(result));
                                   });

            // In case contention somehow caused a different return.  The reference should still be valid if so.
            result = actualReference.Target;
            return DecodeNullObject<TValue>(result);
        }

        public TValue AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
        {
            object result = default(TValue);
            // If we create a new reference we're insured there is a strong reference by parameter "addValue".
            var actualReference =
                _inner.AddOrUpdate(key,
                                   innerKey => new WeakReference(EncodeNullObject(addValue)),
                                   (innerKey, innerReference) =>
                                   {
                                       // By assigning to variable in outer scope we've created a strong reference that will last long enough for outer method to exit.
                                       result = innerReference.Target;
                                       // If the reference is dead, create a new value, else update the existing one.
                                       result = result == null ? addValue : updateValueFactory(innerKey, DecodeNullObject<TValue>(result));
                                       return new WeakReference(EncodeNullObject(result));
                                   });

            // In case contention somehow caused a different return.  The reference should still be valid if so.
            result = actualReference.Target;
            return DecodeNullObject<TValue>(result);
        }

        public bool TryRemove(TKey key, out TValue value)
        {
            value = default(TValue);

            WeakReference wr;
            if (!_inner.TryRemove(key, out wr))
                return false;

            object result = wr.Target;

            if (result == null)
            {
                // Only remove the item if the weak reference still matches.
                ((ICollection<KeyValuePair<TKey, WeakReference>>)_inner).Remove(new KeyValuePair<TKey, WeakReference>(key, wr));
                // The item didn't really exist since the weak reference was already invalid.
                return false;
            }

            value = DecodeNullObject<TValue>(result);
            return true;
        }

        /// <summary>
        /// Since the collection can change internally, it can never be treated as readonly.
        /// </summary>
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly { get { return false; } }

        /// <summary>
        /// Returns a count of the number of items in the dictionary.
        /// </summary>
        /// <remarks>
        /// Since the items in the dictionary are held by weak reference, the count value
        /// cannot be relied upon to guarantee the number of objects that would be discovered via
        /// enumeration. Treat the Count as an estimate only.
        /// </remarks>
        public int Count
        {
            get
            {
                CleanAbandonedItems();
                return _inner.Count;
            }
        }

        private void CleanAbandonedItems()
        {
            foreach (var kvp in _inner.Where(kvp => kvp.Value.Target == null))
                ((ICollection<KeyValuePair<TKey, WeakReference>>)_inner).Remove(kvp);
        }

        #region Completed (untested) support methods
        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            TValue dummy;

            if (TryGetValue(key, out dummy))
                throw new ArgumentException(Resources.KeyAlreadyPresent);

            ((IDictionary<TKey, WeakReference>)_inner).Add(key, new WeakReference(EncodeNullObject(value)));
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            ((IDictionary<TKey, TValue>)this).Add(item.Key, item.Value);
        }

        /// <summary>
        /// Removes an item from the dictionary.
        /// </summary>
        /// <param name="key">The key of the item to be removed.</param>
        /// <returns>Returns true if the key was in the dictionary; return false otherwise.</returns>
        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            TValue dummy;
            return TryRemove(key, out dummy);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            WeakReference wr;
            if (!_inner.TryGetValue(item.Key, out wr))
                return false;

            object result = wr.Target;
            if (!ReferenceEquals(result, item.Value))
                return false;

            // Only remove the item if the weak reference still matches.
            return ((ICollection<KeyValuePair<TKey, WeakReference>>)_inner).Remove(new KeyValuePair<TKey, WeakReference>(item.Key, wr));
        }

        /// <summary>
        /// Retrieves a value from the dictionary.
        /// </summary>
        /// <param name="key">The key to look for.</param>
        /// <returns>The value in the dictionary.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the key does exist in the dictionary.
        /// Since the dictionary contains weak references, the key may have been removed by the
        /// garbage collection of the object.</exception>
        TValue IDictionary<TKey, TValue>.this[TKey key]
        {
            get
            {
                TValue result;

                if (TryGetValue(key, out result))
                    return result;

                throw new KeyNotFoundException();
            }
            set
            {
                AddOrUpdate(key, value, (innerKey, innerValue) => value);
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            TValue value;
            if (!TryGetValue(item.Key, out value))
                return false;
            return ReferenceEquals(item.Value, value);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            this.ToArray().CopyTo(array, arrayIndex);
        }
        

        ICollection<TKey> IDictionary<TKey, TValue>.Keys
        {
            get
            {
                return this.Select(kvp => kvp.Key).ToList().AsReadOnly();
            }
        }

        ICollection<TValue> IDictionary<TKey, TValue>.Values
        {
            get { return this.Select(kvp => kvp.Value).ToList().AsReadOnly(); }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region Null Object Specialiation
        private static readonly object NullObject = new object();

        static TObject DecodeNullObject<TObject>(object innerValue)
        {
            return ReferenceEquals(innerValue, NullObject) ? default(TObject) : (TObject) innerValue;
        }

        static object EncodeNullObject(object value)
        {
            return value ?? NullObject;
        }
        #endregion
    }
}