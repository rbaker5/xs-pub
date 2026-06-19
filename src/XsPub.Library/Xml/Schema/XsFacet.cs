using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;

namespace XsPub.Library.Xml.Schema
{
    public class XsFacet : XsAnnotated
    {
        private static IDictionary<XName, FacetType> _facetsTypes =
            new Dictionary<XName, FacetType>
                {
                    {Xs.FractionDigits, FacetType.FractionDigits},
                    {Xs.Length, FacetType.Length},
                    {Xs.Enumeration, FacetType.Enumeration},
                    {Xs.ExplicitTimezone, FacetType.ExplicitTimezone},
                    {Xs.MaxExclusive, FacetType.MaxExclusive},
                    {Xs.MaxInclusive, FacetType.MaxInclusive},
                    {Xs.MaxLength, FacetType.MaxLength},
                    {Xs.MinExclusive, FacetType.MinExclusive},
                    {Xs.MinInclusive, FacetType.MinInclusive},
                    {Xs.MinLength, FacetType.MinLength},
                    {Xs.Pattern, FacetType.Pattern},
                    {Xs.TotalDigits, FacetType.TotalDigits},
                    {Xs.WhiteSpace, FacetType.WhiteSpace}
                };

        private static IDictionary<FacetType, XName> _facetNames = _facetsTypes.InvertDictionary();

        public static XsFacet Create(XElement element, XsObject parent)
        {
            if (!IsFacetElement(element))
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,
                                                      "Unsupported schema element: {0}", element.Name));

            return new XsFacet(element, parent);
        }

        internal static bool IsFacetElement(XElement element)
        {
            return _facetsTypes.ContainsKey(element.Name);
        }

        private XsFacet(XElement element) : base(element)
        {
        }

        private XsFacet(XElement element, XsObject parent) : base(element, parent)
        {
        }

        public bool IsFixed
        {
            get { return GetAttributeValueInternal(XsA.Fixed, false); }
            set { SetAttributeValueInternal(XsA.Fixed, value, false); }
        }

        public string Value
        {
            get { return GetAttributeValueInternal(XsA.Value, null); }
            set { SetAttributeValueInternal(XsA.Value, value); }
        }

        public FacetType FacetType
        {
            get
            {
                FacetType type;
                if (!_facetsTypes.TryGetValue(Element.Name, out type))
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture,
                                                      "Unsupported facet element: {0}", Element.Name));
                return type;
            }
        }

        protected override bool IsValidAttribute(XName attributeName)
        {
            if (attributeName == XsA.Fixed || attributeName == XsA.Value) return true;
            return base.IsValidAttribute(attributeName);
        }
    }
}