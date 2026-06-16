using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace XsPub.Library.Xml.Schema
{
    /// <summary>
    /// The base class for any element that can contain annotation elements.
    /// </summary>
    public abstract class XsAnnotated : XsObject
    {
        /// <summary>
        /// Initializes an annotated schema object outside or at the root of a tree structure.
        /// </summary>
        /// <param name="element">An element containing the XML representation of an annotated schema object.</param>
        protected XsAnnotated(XElement element)
            : base(element)
        {

        }

        /// <summary>
        /// Initializes an annotated schema object inside a tree structure.
        /// </summary>
        /// <param name="element">An element containing the XML representation of an annotated schema object.</param>
        /// <param name="parent">An appropriate wrapper around <paramref name="element"/>.<see cref="XElement.Parent">Parent</see>.</param>
        protected XsAnnotated(XElement element, XsObject parent)
            : base(element, parent)
        {
        }

        /// <summary>
        /// Gets or sets the string id.
        /// </summary>
        /// <value>
        /// The string id. The default is String.Empty.
        /// </value>
        public String Id
        {
            get { return GetAttributeValueInternal(XsA.Id); }
            set { SetAttributeValueInternal(XsA.Id, value); }
        }
        /// <summary>
        /// Gets the qualified attributes that do not belong to the current schema's target namespace.
        /// </summary>
        public IEnumerable<XAttribute> UnhandledAttributes { get { return GetUnhandledAttributes(); } }

        /// <summary>
        /// Gets or sets the annotation property.
        /// </summary>
        public XsAnnotation Annotation
        {
            get { return GetElement<XsAnnotation>(Xs.Annotation); }
            set { SetElement(Xs.Annotation, value); }
        }
    }
}