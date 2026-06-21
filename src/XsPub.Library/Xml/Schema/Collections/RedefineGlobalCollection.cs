using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace XsPub.Library.Xml.Schema.Collections;

internal class RedefineGlobalCollection
{
    public static IGlobalNamedCollection<TOutput> Create<TOutput>(XsRedefine redefine,
                                                                  INamedCollection<TOutput> localCollection,
                                                                  Func<XsSchema, IGlobalNamedCollection<TOutput>> subValueSelector)
        where TOutput : XsObject, IGlobalNamedObject
    {
        return new RedefineGlobalCollection<TOutput>(redefine, localCollection, subValueSelector);
    }
}

internal class RedefineGlobalCollection<TOutput> : IGlobalNamedCollection<TOutput>
    where TOutput : XsObject, IGlobalNamedObject
{
    protected XsRedefine Redefine { get; private set; }
    protected INamedCollection<TOutput> LocalCollection { get; private set; }
    protected Func<XsSchema, IGlobalNamedCollection<TOutput>> SubValueSelector { get; private set; }

    internal RedefineGlobalCollection(XsRedefine redefine,
                                      INamedCollection<TOutput> localCollection,
                                      Func<XsSchema, IGlobalNamedCollection<TOutput>> subValueSelector)
    {
        Redefine = redefine;
        LocalCollection = localCollection;
        SubValueSelector = subValueSelector;
    }

    public TOutput this[XName name]
    {
        get
        {
            TOutput returnValue = null;
            if (Redefine.Schema.TargetNamespace == name.Namespace)
                returnValue = LocalCollection[name.LocalName];

            return returnValue ?? SubValueSelector(Redefine.Schema)[name];
        }
    }

    #region Implementation of IEnumerable

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public IEnumerator<TOutput> GetEnumerator()
    {
        return LocalCollection.Union(SubValueSelector(Redefine.Schema), new XsObjectNameEqualityComparer()).GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
    /// </returns>
    /// <filterpriority>2</filterpriority>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    #endregion

    private class XsObjectNameEqualityComparer : IEqualityComparer<TOutput>
    {
        #region Implementation of IEqualityComparer<TOutput>

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        /// <param name="x">The first object of type <typeparamref name="TOutput"/> to compare.</param><param name="y">The second object of type <typeparamref name="TOutput"/> to compare.</param>
        public bool Equals(TOutput x, TOutput y)
        {
            return x.QualifiedName == y.QualifiedName;
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <returns>
        /// A hash code for the specified object.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> for which a hash code is to be returned.</param><exception cref="T:System.ArgumentNullException">The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.</exception>
        public int GetHashCode(TOutput obj)
        {
            return obj.QualifiedName.GetHashCode();
        }

        #endregion
    }
}