using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using XsPub.Library.Xml.Schema.Collections;

namespace XsPub.Library.Xml.Schema
{
    public class XsAttributeGroup : XsAnnotated, IGlobalNamedObject
    {
        public XsAttributeGroup(XElement element)
            : base(element)
        {
        }

        public XsAttributeGroup(XElement element, XsObject parent)
            : base(element, parent)
        {
        }

        #region Name attributes
        /// <summary>
        /// Gets or sets the name of the element.
        /// </summary>
        /// <value>
        /// The name of the element. The default is String.Empty.
        /// </value>
        public string Name
        {
            get { return GetAttributeValueInternal(XsA.Name); }
            set { SetAttributeValueInternal(XsA.Name, value); }
        }

        public XName QualifiedName { get { return GetGlobalQualifiedName(XsA.Name); } }
        #endregion

        public XsAnyAttribute AnyAttribute
        {
            get { return GetElement<XsAnyAttribute>(Xs.AnyAttribute); }
            set { SetElement(Xs.AnyAttribute, value); }
        }

        private ICollection<XsObject> _attributes;
        private static readonly IEnumerable<XName> _attributestElementNames = new[] { Xs.Attribute, Xs.AttributeGroup };
        public ICollection<XsObject> Attributes
        {
            get
            {
                if (_attributes == null) _attributes = XsCollection.Create<XsObject>(this, _attributestElementNames);
                return _attributes;
            }
        }

        private static readonly IEnumerable<XName> _particleElementNames = new[] { Xs.Sequence, Xs.Choice, Xs.All };
        public XsParticle Particle
        {
            get { return GetExclusiveOrElement<XsParticle>(_particleElementNames); }
            set { SetExclusiveOrElement(_particleElementNames, value); }
        }

        //public XsAttributeGroup RedefinedAttributeGroup { get; }
    }
}