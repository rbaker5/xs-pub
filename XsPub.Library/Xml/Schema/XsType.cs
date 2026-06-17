using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace XsPub.Library.Xml.Schema
{
    public abstract class XsType : XsAnnotated, IGlobalNamedObject
    {
        protected XsType(XElement element) : base(element)
        {
        }

        protected XsType(XElement element, XsObject parent) : base(element, parent)
        {
        }

        // XsType BaseXmlSchemaType {get;}
        //public XsType DataType {get;}
        public abstract XmlSchemaDerivationMethod DerivedBy {get;}
        public virtual bool IsMixed { get { return false; } set { }}
        //public XmlTypeCode TypeCode
        //{
        //    get
        //    {
        //        if (this == XsComplexType.AnyType)
        //        {
        //            return XmlTypeCode.Item;
        //        }
        //        if (Datatype == null)
        //        {
        //            return XmlTypeCode.None;
        //        }
        //        return Datatype.TypeCode;
        //    }
        //}


        public string Name
        {
            get { return GetAttributeValueInternal(XsA.Name); }
            set { SetAttributeValueInternal(XsA.Name, value); }
        }

        public XName QualifiedName { get { return GetGlobalQualifiedName(XsA.Name); } }

        public XmlSchemaDerivationMethod Final
        {
            get { return GetAttributeValueInternal(XsA.Final, XmlSchemaDerivationMethod.None); }
            set { SetAttributeValueInternal(XsA.Final, value, XmlSchemaDerivationMethod.None); }
        }

        public XmlSchemaDerivationMethod FinalResolved
        {
            get { return ResolveDerivationMethod(XsA.Final, FinalResolvedFilter, schema => schema.FinalDefault); }
        }

        /// <summary>
        /// Internal property to specifies the maximum value of Final for a derived type.
        /// </summary>
        internal abstract XmlSchemaDerivationMethod FinalResolvedFilter { get; }

        public override string ToString()
        {
            return Name;
        }
    }
}