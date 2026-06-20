using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace XsPub.Library.Xml.Schema.Collections
{
    internal static class SchemaCollection
    {
        public static IGlobalNamedCollection<TOutput> CreateGlobal<TOutput>(XsSchema schema,
                                                                            INamedCollection<TOutput> localCollection,
                                                                            Func<XsSchema, IGlobalNamedCollection<TOutput>> subValueSelector,
                                                                            Func<XsRedefine, IGlobalNamedCollection<TOutput>> redefineValueSelector)
            where TOutput : XsObject, IGlobalNamedObject
        {
            return new SchemaCollection<TOutput>(schema, localCollection, subValueSelector, redefineValueSelector, false);
        }


        public static IGlobalNamedCollection<TOutput> CreateLocal<TOutput>(XsSchema schema,
                                                                           INamedCollection<TOutput> localCollection,
                                                                           Func<XsSchema, IGlobalNamedCollection<TOutput>> subValueSelector,
                                                                           Func<XsRedefine, IGlobalNamedCollection<TOutput>> redefineValueSelector)
            where TOutput : XsObject, IGlobalNamedObject
        {
            return new SchemaCollection<TOutput>(schema, localCollection, subValueSelector, redefineValueSelector, true);
        }
    }

    internal class SchemaCollection<TOutput> : IGlobalNamedCollection<TOutput>
        where TOutput : XsObject, IGlobalNamedObject
    {
        protected XsSchema Schema { get; private set; }
        protected INamedCollection<TOutput> LocalCollection { get; private set; }
        protected Func<XsSchema, IGlobalNamedCollection<TOutput>> SubValueSelector { get; private set; }
        protected Func<XsRedefine, IGlobalNamedCollection<TOutput>> RedefineValueSelector { get; private set; }
        protected bool IsLocal { get; private set; }

        internal SchemaCollection(XsSchema schema, 
                                  INamedCollection<TOutput> localCollection,
                                  Func<XsSchema, IGlobalNamedCollection<TOutput>> subValueSelector,
                                  Func<XsRedefine, IGlobalNamedCollection<TOutput>> redefineValueSelector, bool isLocal)
        {
            Schema = schema;
            LocalCollection = localCollection;
            SubValueSelector = subValueSelector;
            RedefineValueSelector = redefineValueSelector;
            IsLocal = isLocal;
        }

        public TOutput this[XName name]
        {
            get
            {
                TOutput returnValue = null;
                if (Schema.TargetNamespace == name.Namespace)
                    returnValue = LocalCollection[name.LocalName];
                else if (IsLocal) return null;

                foreach (var subValue in Schema.Includes
                    .Where(include => include.Schema.TargetNamespace == name.NamespaceName)
                    .Select(include => getSubValue(name, include)))
                {
                    if (returnValue != null && subValue != null)
                        throw new InvalidOperationException("Two objects with conflicting names found.");

                    if (returnValue == null) returnValue = subValue;
                }
                
                return returnValue;
            }
        }

        private TOutput getSubValue(XName name, XsExternal external)
        {
            return getSubValues(external)[name];
        }

        private IGlobalNamedCollection<TOutput> getSubValues(XsExternal external)
        {
            var redefine = external as XsRedefine;
            if (redefine != null && RedefineValueSelector != null) return RedefineValueSelector(redefine);
            return SubValueSelector(external.Schema);
        }

        #region Implementation of IEnumerable

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<TOutput> GetEnumerator()
        {
            IEnumerable<XsExternal> externals = Schema.Includes;
            if (IsLocal) 
                externals = externals.Where(external => external.Schema.TargetNamespace == Schema.TargetNamespace);

            var includedOutputs = externals.SelectMany<XsExternal, TOutput>(getSubValues);
            return LocalCollection.Concat(includedOutputs).GetEnumerator();
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
    }
}