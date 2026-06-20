using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace XsPub.Library.Xml.Schema.Collections
{
    internal class SchemaCollectionSet
    {
        internal static SchemaCollectionSet<T> Create<T>(XsSchema schema, XName elementName,
                                                         Func<XsSchema, IGlobalNamedCollection<T>> subValueSelector,
                                                         Func<XsRedefine, IGlobalNamedCollection<T>> redefineValueSelector)
            where T : XsObject, IGlobalNamedObject
        {
            return create(schema, subValueSelector, redefineValueSelector,
                          new(() => NamedCollection.Create<T>(schema, elementName)));
        }

        internal static SchemaCollectionSet<T> Create<T>(XsSchema schema, IEnumerable<XName> elementNames,
                                                         Func<XsSchema, IGlobalNamedCollection<T>> subValueSelector,
                                                         Func<XsRedefine, IGlobalNamedCollection<T>> redefineValueSelector)
            
            where T : XsObject, IGlobalNamedObject
        {
            return create(schema, subValueSelector, redefineValueSelector,
                          new(() => NamedCollection.Create<T>(schema, elementNames)));
        }

        private static SchemaCollectionSet<T> create<T>(XsSchema schema, 
                                                        Func<XsSchema, IGlobalNamedCollection<T>> subValueSelector,
                                                        Func<XsRedefine, IGlobalNamedCollection<T>> redefineValueSelector, 
                                                        Lazy<INamedCollection<T>> local)

            where T : XsObject, IGlobalNamedObject
        {
            var set = new SchemaCollectionSet<T>();
            set.Init(local,
                     new(() => SchemaCollection.CreateGlobal(schema, set.Local, subValueSelector, redefineValueSelector)),
                     new(() => SchemaCollection.CreateLocal(schema, set.Local, subValueSelector, redefineValueSelector)));
            return set;
        }
    }

    internal class SchemaCollectionSet<T>
        where T : XsObject, IGlobalNamedObject
    {
        private Lazy<INamedCollection<T>> _local;
        private Lazy<IGlobalNamedCollection<T>> _global;
        private Lazy<IGlobalNamedCollection<T>> _general;

        /// <summary>
        /// All locally defined objects, referenced by their local, unqualified names.
        /// </summary>
        internal INamedCollection<T> Local { get { return _local.Value; }}
        /// <summary>
        /// The Global collection includes objects from this schema all objects from any
        /// referenced schema, including those recursively referenced.
        /// </summary>
        internal IGlobalNamedCollection<T> Global { get { return _global.Value; } }

        /// <summary>
        /// The "General" collection is only locally defined schema objects.  It is
        /// different from <see cref="Local"/> by requiring a qualified name to lookup.
        /// </summary>
        internal IGlobalNamedCollection<T> General { get { return _general.Value; } }

        internal void Init(Lazy<INamedCollection<T>> local, Lazy<IGlobalNamedCollection<T>> global, Lazy<IGlobalNamedCollection<T>> general)
        {
            _local = local;
            _global = global;
            _general = general;
        }
    }
}