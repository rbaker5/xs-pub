using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace XsPub.Library.Xml
{
    public static class Namespaces
    {
        public static readonly XNamespace Xs = XNamespace.Get(@"http://www.w3.org/2001/XMLSchema");
        public static readonly XNamespace Wsdl = XNamespace.Get(@"http://schemas.xmlsoap.org/wsdl/");
    }

    public static class Xs
    {
        // Order may seem funny.. taken from ENTITY list in schema DTD.
        public static XName Schema { get { return Namespaces.Xs.GetName("schema"); } }
        public static XName DefaultOpenContent { get { return Namespaces.Xs.GetName("defaultOpenContent"); } }
        public static XName ComplexType { get { return Namespaces.Xs.GetName("complexType"); } }
        public static XName ComplexContent { get { return Namespaces.Xs.GetName("complexContent"); } }
        public static XName OpenContent { get { return Namespaces.Xs.GetName("openContent"); } }
        public static XName SimpleContent { get { return Namespaces.Xs.GetName("simpleContent"); } }
        public static XName Extension { get { return Namespaces.Xs.GetName("extension"); } }
        public static XName Element { get { return Namespaces.Xs.GetName("element"); } }
        public static XName Alternative { get { return Namespaces.Xs.GetName("alternative"); } }
        public static XName Unique { get { return Namespaces.Xs.GetName("unique"); } }
        public static XName Key { get { return Namespaces.Xs.GetName("key"); } }
        public static XName Keyref { get { return Namespaces.Xs.GetName("keyref"); } }
        public static XName Selector { get { return Namespaces.Xs.GetName("selector"); } }
        public static XName Field { get { return Namespaces.Xs.GetName("field"); } }
        public static XName Group { get { return Namespaces.Xs.GetName("group"); } }
        public static XName All { get { return Namespaces.Xs.GetName("all"); } }
        public static XName Choice { get { return Namespaces.Xs.GetName("choice"); } }
        public static XName Sequence { get { return Namespaces.Xs.GetName("sequence"); } }
        public static XName Any { get { return Namespaces.Xs.GetName("any"); } }
        public static XName AnyAttribute { get { return Namespaces.Xs.GetName("anyAttribute"); } }
        public static XName Attribute { get { return Namespaces.Xs.GetName("attribute"); } }
        public static XName AttributeGroup { get { return Namespaces.Xs.GetName("attributeGroup"); } }
        public static XName Import { get { return Namespaces.Xs.GetName("import"); } }
        public static XName Include { get { return Namespaces.Xs.GetName("include"); } }
        public static XName Redefine { get { return Namespaces.Xs.GetName("redefine"); } }
        public static XName Override { get { return Namespaces.Xs.GetName("override"); } }
        public static XName Notation { get { return Namespaces.Xs.GetName("notation"); } }
        public static XName Assert { get { return Namespaces.Xs.GetName("assert"); } }

        public static XName Annotation { get { return Namespaces.Xs.GetName("annotation"); } }
        public static XName AppInfo { get { return Namespaces.Xs.GetName("appinfo"); } }
        public static XName Documentation { get { return Namespaces.Xs.GetName("documentation"); } }

        // And these come from part 2, data type definitions: http://www.w3.org/TR/xmlschema11-2/

        public static XName SimpleType { get { return Namespaces.Xs.GetName("simpleType"); } }
        public static XName Restriction { get { return Namespaces.Xs.GetName("restriction"); } }
        public static XName List { get { return Namespaces.Xs.GetName("list"); } }
        public static XName Union { get { return Namespaces.Xs.GetName("union"); } }
        public static XName MaxExclusive { get { return Namespaces.Xs.GetName("maxExclusive"); } }
        public static XName MinExclusive { get { return Namespaces.Xs.GetName("minExclusive"); } }
        public static XName MaxInclusive { get { return Namespaces.Xs.GetName("maxInclusive"); } }
        public static XName MinInclusive { get { return Namespaces.Xs.GetName("minInclusive"); } }
        public static XName TotalDigits { get { return Namespaces.Xs.GetName("totalDigits"); } }
        public static XName FractionDigits { get { return Namespaces.Xs.GetName("fractionDigits"); } }
        public static XName MaxScale { get { return Namespaces.Xs.GetName("maxScale"); } }
        public static XName MinScale { get { return Namespaces.Xs.GetName("minScale"); } }
        public static XName Length { get { return Namespaces.Xs.GetName("length"); } }
        public static XName MinLength { get { return Namespaces.Xs.GetName("minLength"); } }
        public static XName MaxLength { get { return Namespaces.Xs.GetName("maxLength"); } }
        public static XName Enumeration { get { return Namespaces.Xs.GetName("enumeration"); } }
        public static XName WhiteSpace { get { return Namespaces.Xs.GetName("whiteSpace"); } }
        public static XName Pattern { get { return Namespaces.Xs.GetName("pattern"); } }
        public static XName Assertion { get { return Namespaces.Xs.GetName("assertion"); } }
        public static XName ExplicitTimezone { get { return Namespaces.Xs.GetName("explicitTimezone"); } }
    }

    /// <summary>
    /// A list of attributes from the schema definition.
    /// </summary>
    internal static class XsA
    {
        public static readonly XName Id = "id";
        public static readonly XName Ref = "ref";

        public static readonly XName TargetNamespace = "targetNamespace";
        public static readonly XName AttributeFormDefault = "attributeFormDefault";
        public static readonly XName ElementFormDefault = "elementFormDefault";
        public static readonly XName BlockDefault = "blockDefault";
        public static readonly XName FinalDefault = "finalDefault";
        public static readonly XName Version = "version";
        public static readonly XName SchemaLocation = "schemaLocation";


        public static readonly XName Block = "block";
        public static readonly XName Final = "final";
        public static readonly XName Abstract = "abstract";
        public static readonly XName Default = "default";
        public static readonly XName Fixed = "fixed";

        public static readonly XName Form = "form";
        public static readonly XName Nillable = "nillable";
        public static readonly XName SubstitutionGroup = "substitutionGroup";

        public static readonly XName Type = "type";
        public static readonly XName MaxOccurs = "maxOccurs";
        public static readonly XName MinOccurs = "minOccurs";
        public static readonly XName ItemType = "itemType";
        public static readonly XName Base = "base";
        public static readonly XName Name = "name";
        public static readonly XName Namespace = "namespace";
        public static readonly XName ProcessContents = "processContents";
        public static readonly XName Source = "source";
        public static readonly XName Use = "use";
        public static readonly XName Mixed = "mixed";
        public static readonly XName Value = "value";
        public static readonly XName Public = "public";
        public static readonly XName System = "system";
    }

    public static class Wsdl
    {
        public static XName Definitions { get { return Namespaces.Wsdl.GetName("definitions"); } }
        public static XName Types { get { return Namespaces.Xs.GetName("types"); } }
        public static XName Import { get { return Namespaces.Xs.GetName("import"); } }
    }
}