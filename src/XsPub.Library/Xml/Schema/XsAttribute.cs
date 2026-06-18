using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace XsPub.Library.Xml.Schema
{
    public class XsAttribute : XsAnnotated, IGlobalNamedObject
    {
        public XsAttribute(XElement element) : base(element)
        {
            if (element.Name != Xs.Element) throw new ArgumentException($"Expected {nameof(Xs.Element)}, got {element.Name}.", nameof(element));
        }

        public XsAttribute(XElement element, XsObject parent) : base(element, parent)
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

        public XName QualifiedName { get { return GetLocalGlobalOrReferencedName(XsA.Name, XsA.Ref, Form, xschema => xschema.AttributeFormDefault); } }
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

        public XName RefName
        {
            get { return GetQualifiedName(XsA.Ref); }
            set { SetQualifiedName(XsA.Ref, value); }
        }

        public XmlSchemaUse Use
        {
            get { return GetAttributeValueInternal(XsA.Use, XmlSchemaUse.None); }
            set { SetAttributeValueInternal(XsA.Use, value, XmlSchemaUse.None); }
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

        public XsSimpleType SchemaType 
        {
            get { return GetElement<XsSimpleType>(Xs.SimpleType); }
            set
            {
                SetElement(Xs.SimpleType, value);
            }
        }

        // TODO: Implement Type Resolution
        //public XsType AttributeSchemaType
        //{
        //    get
        //    {
        //        return GetSchema().Types.Where(type => type.QualifiedName == SchemaTypeName);
        //    }
        //}
        #endregion

        private static readonly HashSet<XName> _validAttributes = new HashSet<XName> { XsA.Default, XsA.Fixed, XsA.Form,XsA.Name, XsA.Type, XsA.Default, XsA.Ref };
        protected override bool IsValidAttribute(XName attributeName)
        {
            if (_validAttributes.Contains(attributeName)) return true;
            return base.IsValidAttribute(attributeName);
        }
    }
}