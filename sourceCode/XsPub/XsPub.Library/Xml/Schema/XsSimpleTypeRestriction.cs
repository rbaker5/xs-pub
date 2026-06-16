using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using XsPub.Library.Xml.Schema.Collections;

namespace XsPub.Library.Xml.Schema
{
    /// <summary>
    /// Represents the restriction element for simple types from XML Schema as
    /// specified by the World Wide Web Consortium (W3C). This class can be used
    /// restricting simpleType element.
    /// </summary>
    public class XsSimpleTypeRestriction : XsSimpleTypeContent
    {
        /// <summary>
        /// Initializes a restriction schema object outside or at the root of a tree structure.
        /// </summary>
        /// <param name="element">An element containing the XML representation of a restriction schema object.</param>
        public XsSimpleTypeRestriction(XElement element)
            : base(element)
        {
        }

        /// <summary>
        /// Initializes a restriction schema object inside a tree structure.
        /// </summary>
        /// <param name="element">An element containing the XML representation of a restriction schema object.</param>
        /// <param name="parent">An appropriate wrapper around <paramref name="element"/>.<see cref="XElement.Parent">Parent</see>.</param>
        public XsSimpleTypeRestriction(XElement element, XsObject parent)
            : base(element, parent)
        {
        }

        /// <summary>Gets or sets information on the base type.</summary>
        /// <value>The base type for the simpleType element.</value>
        public XsSimpleType BaseType
        {
            get { return GetElement<XsSimpleType>(Xs.SimpleType); }
            set { SetElement(Xs.SimpleType, value); }
        }

        /// <summary>Gets or sets the name of the qualified base type.</summary>
        /// <value>The qualified name of the simple type restriction base type.</value>
        public XName BaseTypeName
        {
            get { return GetQualifiedName(XsA.Base); }
            set { SetQualifiedName(XsA.Base, value); }
        }

        private ICollection<XsFacet> _facets;

        private static readonly IEnumerable<XName> _facetElementNames =
            new[]
                {
                    Xs.TotalDigits, Xs.FractionDigits,
                    Xs.MaxExclusive, Xs.MaxInclusive,
                    Xs.MinExclusive, Xs.MinInclusive,
                    Xs.Length, Xs.MaxLength, Xs.MinLength,
                    Xs.Enumeration, Xs.Pattern,
                    Xs.WhiteSpace
                };

        /// <summary>
        /// A modifiable collection of schema restriction facets.
        /// </summary>
        /// <value>
        /// One of the following facet classes: LengthFacet, MinLengthFacet,
        /// MaxLengthFacet, PatternFacet, EnumerationFacet, MaxInclusiveFacet,
        /// MaxExclusiveFacet, MinInclusiveFacet, MinExclusiveFacet, FractionDigitsFacet,
        /// TotalDigitsFacet, WhiteSpaceFacet.
        /// </value>
        public ICollection<XsFacet> Facets
        {
            get
            {
                if (_facets == null) _facets = XsCollection.Create<XsFacet>(this, _facetElementNames);
                return _facets;
            }
        }

        protected override bool IsValidAttribute(XName attributeName)
        {
            if (attributeName == XsA.Base) return true;
            return base.IsValidAttribute(attributeName);
        }
    }
}