using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using System.Xml.Schema;

namespace XsPub.Library.Xml.Schema
{
    /// <summary>
    /// Represents the simpleType element for simple content from XML Schema as
    /// specified by the World Wide Web Consortium (W3C). This class defines a simple
    /// type. Simple types can specify information and constraints for the value of
    /// attributes or elements with text-only content.
    /// </summary>
    public class XsSimpleType : XsType
    {
        internal sealed override XmlSchemaDerivationMethod FinalResolvedFilter { get { return XmlSchemaDerivationMethod.Union | XmlSchemaDerivationMethod.List | XmlSchemaDerivationMethod.Restriction; } }

        /// <summary>
        /// Initializes a simpleType schema object outside or at the root of a tree structure.
        /// </summary>
        /// <param name="element">An element containing the XML representation of a simpleType schema object.</param>
        public XsSimpleType(XElement element)
            : base(element)
        {
        }

        /// <summary>
        /// Initializes a simpleType schema object inside a tree structure.
        /// </summary>
        /// <param name="element">An element containing the XML representation of a simpleType schema object.</param>
        /// <param name="parent">An appropriate wrapper around <paramref name="element"/>.<see cref="XElement.Parent">Parent</see>.</param>
        public XsSimpleType(XElement element, XsObject parent)
            : base(element, parent)
        {
        }

        /// <exception cref="InvalidOperationException"><c>InvalidOperationException</c>.</exception>
        public override XmlSchemaDerivationMethod DerivedBy
        {
            get
            {
                if (Content is XsSimpleTypeList) return XmlSchemaDerivationMethod.List;
                if (Content is XsSimpleTypeRestriction) return XmlSchemaDerivationMethod.Restriction;
                if (Content is XsSimpleTypeUnion) return XmlSchemaDerivationMethod.Union;

                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture,
                          "Unsupported content: {0}", Content == null ? "null" : Content.Element.ToString()));
            }
        }

        private static readonly IEnumerable<XName> _contentElementNames = new[] { Xs.Restriction, Xs.List, Xs.Union };

        /// <summary>
        /// Gets or sets one of <see cref="XsSimpleTypeUnion"/>, <see cref="XsSimpleTypeList"/>, or <see cref="XsSimpleTypeRestriction"/>.
        ///  </summary>
        /// <value>
        /// One of <see cref="XsSimpleTypeUnion"/>, <see cref="XsSimpleTypeList"/>, or <see cref="XsSimpleTypeRestriction"/>.
        /// </value>
        public XsSimpleTypeContent Content
        {
            get { return GetExclusiveOrElement<XsSimpleTypeContent>(_contentElementNames) ; }
            set { SetExclusiveOrElement(_contentElementNames, value); }
        }
    }
}