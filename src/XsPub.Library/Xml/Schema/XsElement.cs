using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using XsPub.Library.Xml.Schema.Collections;

namespace XsPub.Library.Xml.Schema
{
    public class XsElement : XsParticle, IGlobalNamedObject
    {
        private const XmlSchemaDerivationMethod BlockResolvedFilter = XmlSchemaDerivationMethod.Restriction | XmlSchemaDerivationMethod.Extension | XmlSchemaDerivationMethod.Substitution;
        private const XmlSchemaDerivationMethod FinalResolvedFilter = XmlSchemaDerivationMethod.Restriction | XmlSchemaDerivationMethod.Extension;

        public XsElement(XElement element) : base(element)
        {
            if (element.Name != Xs.Element) throw new ArgumentException($"Expected {nameof(Xs.Element)}, got {element.Name}.", nameof(element));
        }

        public XsElement(XElement element, XsObject parent) : base(element, parent)
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

        public XName QualifiedName { get { return GetLocalGlobalOrReferencedName(XsA.Name, XsA.Ref, Form, xschema => xschema.ElementFormDefault); } }
        #endregion
        
        /// <summary>
        /// Gets or sets the default value of the element if its content is a simple type or content of the element is textOnly.
        /// </summary>
        /// <value>
        /// The default value for the element.  The default is a null reference.  Optional.
        /// </value>
        public string DefaultValue
        {
            get { return GetAttributeValueInternal(XsA.Default); }
            set { SetAttributeValueInternal(XsA.Default, value); }
        }

        public string FixedValue
        {
            get { return GetAttributeValueInternal(XsA.Fixed); }
            set { SetAttributeValueInternal(XsA.Fixed, value); }
        }

        public XmlSchemaForm Form
        {
            get { return GetAttributeValueInternal(XsA.Form, XmlSchemaForm.None); }
            set { SetAttributeValueInternal(XsA.Form, value, XmlSchemaForm.None); }
        }

        public bool IsAbstract
        {
            get { return GetAttributeValueInternal(XsA.Abstract, false); }
            set { SetAttributeValueInternal(XsA.Abstract, value, false); }
        }

        public bool IsNillable
        {
            get { return GetAttributeValueInternal(XsA.Nillable, false); }
            set { SetAttributeValueInternal(XsA.Nillable, value, false); }
        }

        public XName RefName
        {
            get { return GetQualifiedName(XsA.Ref); }
            set { SetQualifiedName(XsA.Ref, value); }
        }

        public XName SubstitutionGroup
        {
            get { return GetQualifiedName(XsA.SubstitutionGroup); }
            set { SetQualifiedName(XsA.SubstitutionGroup, value); }
        }

        public XmlSchemaDerivationMethod Block
        {
            get { return GetAttributeValueInternal(XsA.Block, XmlSchemaDerivationMethod.None); }
            set { SetAttributeValueInternal(XsA.Block, value, XmlSchemaDerivationMethod.None); }
        }

        public XmlSchemaDerivationMethod BlockResolved
        {
            get { return ResolveDerivationMethod(XsA.Block, BlockResolvedFilter, schema => schema.BlockDefault);}
        }

        #region Type Attributes
        public XName SchemaTypeName
        {
            get { return GetQualifiedName(XsA.Type); }
            set
            {
                SetQualifiedName(XsA.Type, value);
            }
        }

        /// <summary>
        /// Gets or sets the anonymous/embedded type of the element.  This can either be a complex type or a simple type.
        /// </summary>
        public XsType SchemaType 
        {
            get { return GetElement<XsType>(Xs.ComplexType) ?? GetElement<XsType>(Xs.SimpleType); } 
            set
            {
                if (value is XsComplexType)
                    SetElement(Xs.ComplexType, value);
                else
                    SetElement(Xs.SimpleType, value);
            }
        }

        // TODO: Implement Type Resolution
        //public XsType ElementSchemaType
        //{
        //    get
        //    {
        //        return GetSchema().Types.Where(type => type.QualifiedName == SchemaTypeName);
        //    }
        //}
        #endregion

        public XmlSchemaDerivationMethod Final
        {
            get { return GetAttributeValueInternal(XsA.Final, XmlSchemaDerivationMethod.None); }
            set { SetAttributeValueInternal(XsA.Final, value, XmlSchemaDerivationMethod.None); }
        }

        public XmlSchemaDerivationMethod FinalResolved
        {
            get { return ResolveDerivationMethod(XsA.Final, FinalResolvedFilter, schema => schema.FinalDefault);}
        }

        private ICollection<XsIdentityConstraint> _constraints;
        private static readonly IEnumerable<XName> _constraintElementNames = new[] {Xs.Key, Xs.Keyref, Xs.Unique};
        public ICollection<XsIdentityConstraint> Constraints
        {
            get
            {
                if (_constraints == null) _constraints = XsCollection.Create<XsIdentityConstraint>(this, _constraintElementNames);
                return _constraints;
            }
        }

    }
}