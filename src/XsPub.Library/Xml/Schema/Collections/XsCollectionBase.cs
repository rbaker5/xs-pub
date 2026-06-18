using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace XsPub.Library.Xml.Schema.Collections
{
    internal abstract class XsCollectionBase<TOutput> : IList<TOutput>
        where TOutput : XsObject
    {
        protected XsObject Parent { get; private set; }
        protected Func<IEnumerable<XElement>> ValueSelector { get; private set;}

        protected XsCollectionBase(XsObject parent, Func<IEnumerable<XElement>> valueSelector)
        {
            ArgumentNullException.ThrowIfNull(parent);
            ArgumentNullException.ThrowIfNull(valueSelector);
            if (parent.Element == null) throw new ArgumentException("parent.Element must not be null.", nameof(parent));

            Parent = parent;
            ValueSelector = valueSelector;
        }

        #region Implementation of IEnumerable

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public virtual IEnumerator<TOutput> GetEnumerator()
        {
            return ValueSelector().Select<XElement, XsObject>(CreateSchemaObject).Cast<TOutput>().GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Implementation of ICollection<TOutput>

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        public void Add(TOutput item)
        {
            //ArgumentNullException.ThrowIfNull(item);
            //Contract.Requires<ArgumentException>(item.Element != null);

            Parent.Element.Add(item.Element);
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only. </exception>
        public void Clear()
        {
            ValueSelector().Remove();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        public bool Contains(TOutput item)
        {
            //ArgumentNullException.ThrowIfNull(item);
            //Contract.Requires<ArgumentException>(item.Element != null);

            return ValueSelector().Contains(item.Element);
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="array"/> is multidimensional.-or-<paramref name="arrayIndex"/> is equal to or greater than the length of <paramref name="array"/>.-or-The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.-or-Type <paramref name="T"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.</exception>
        public void CopyTo(TOutput[] array, int arrayIndex)
        {
            ValueSelector().ToArray();
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the 
        /// <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the 
        /// <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// This method also returns false if <paramref name="item"/> is not found in the
        /// original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <param name="item">
        /// The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </exception>
        public bool Remove(TOutput item)
        {
            //ArgumentNullException.ThrowIfNull(item);
            //Contract.Requires<ArgumentException>(item.Element != null);

            if (item.Element.Parent != Parent.Element) return false;

            item.Element.Remove();
            return true;
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        public int Count
        {
            get { return ValueSelector().Count(); }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
        /// </returns>
        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region Implementation of IList<T>

        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </summary>
        /// <returns>
        /// The index of <paramref name="item"/> if found in the list; otherwise, -1.
        /// </returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        public abstract int IndexOf(TOutput item);

        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param><param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.</exception>
        public void Insert(int index, TOutput item)
        {
            //Contract.Requires<ArgumentOutOfRangeException>(index >= 0);
            //ArgumentNullException.ThrowIfNull(item);
            //Contract.Requires<ArgumentException>(item.Element != null);

            ValueSelector().ElementAt(index).AddAfterSelf(item.Element);
        }

        /// <summary>
        /// Removes the <see cref="T:System.Collections.Generic.IList`1"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.</exception>
        public void RemoveAt(int index)
        {
            ValueSelector().ElementAt(index).Remove();
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <returns>
        /// The element at the specified index.
        /// </returns>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception>
        /// <exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IList`1"/> is read-only.</exception>
        public TOutput this[int index]
        {
            get
            {
                //Contract.Requires<ArgumentOutOfRangeException>(index >= 0);
                return CreateSchemaObject(ValueSelector().ElementAt(index));
            }
            set
            {
                //Contract.Requires<ArgumentOutOfRangeException>(index >= 0);
                //ArgumentNullException.ThrowIfNull(value);
                //Contract.Requires<ArgumentException>(value.Element != null);
                ValueSelector().ElementAt(index).ReplaceWith(value.Element);
            }
        }

        #endregion

        protected TOutput CreateSchemaObject(XElement element) 
        {
            return XsObjectFactory.Create<TOutput>(element, Parent);
        }
    }
}