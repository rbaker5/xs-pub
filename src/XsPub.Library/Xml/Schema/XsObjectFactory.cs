using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using XsPub.Library.Utility;

namespace XsPub.Library.Xml.Schema;

internal class XsObjectFactory
{
    private static readonly ConcurrentWeakRefDictionary<XElement, XsObject> ActiveObjects =
        new ConcurrentWeakRefDictionary<XElement, XsObject>();

    internal static T Create<T>(XElement element, XsObject parent = null) where T : XsObject
    {
        var schemaObject = Create(element, parent);
        if (schemaObject == null) return null;
        var typedObject = schemaObject as T;
        if (typedObject == null)
            throw new ArgumentException(String.Format(CultureInfo.InvariantCulture,
                                                      "Invalid schema object type: {0}, expected: {1}", element.Name, getNameForType(typeof(T))));
        return typedObject;
    }

    internal static T Create<T>(XElement element, Func<XElement, XsObject> valueFactory) where T : XsObject
    {
        ArgumentNullException.ThrowIfNull(valueFactory);

        var schemaObject = Create(element, valueFactory);
        if (schemaObject == null) return null;
        var typedObject = schemaObject as T;
        if (typedObject == null)
            throw new ArgumentException(String.Format(CultureInfo.InvariantCulture,
                                                      "Invalid schema object type: {0}, expected: {1}", element.Name, getNameForType(typeof(T))));
        return typedObject;
    }

    internal static XsObject Create(XElement element, XsObject parent)
    {
        return Create(element, innerElement => createInternal(innerElement, parent));
    }

    /// <summary>
    /// Creates an object if it does not already exist.  This overload is used for schema creation.
    /// </summary>
    /// <param name="element"></param>
    /// <param name="valueFactory"></param>
    /// <returns></returns>
    internal static XsObject Create(XElement element, Func<XElement, XsObject> valueFactory)
    {
        ArgumentNullException.ThrowIfNull(valueFactory);

        if (element == null) return null;
        return ActiveObjects.GetOrAdd(element, valueFactory);
    }

    private static XsObject createInternal(XElement element, XsObject parent)
    {
        ArgumentNullException.ThrowIfNull(element);

        if (XsFacet.IsFacetElement(element)) return XsFacet.Create(element, parent);
        if (element.Name == Xs.All) return new XsAll(element, parent);
        //if (element.Name == Xs.Alternative) return new Alternative(element, parent);
        if (element.Name == Xs.Annotation) return new XsAnnotation(element, parent);
        if (element.Name == Xs.Any) return new XsAny(element, parent);
        if (element.Name == Xs.AnyAttribute) return new XsAnyAttribute(element, parent);
        if (element.Name == Xs.AppInfo) return new XsAppInfo(element, parent);
        //if (element.Name == Xs.Assert) return new Assert(element, parent);
        //if (element.Name == Xs.Assertion) return new Assertion(element, parent);
        if (element.Name == Xs.Attribute) return new XsAttribute(element, parent);
        if (element.Name == Xs.AttributeGroup)
        {
            if (parent is XsAttributeGroup || parent is XsComplexType ||
               (parent == null && element.Parent.IfNotNull(p => new[] { Xs.AttributeGroup, Xs.ComplexType }.Contains(p.Name)))) 
                return new XsAttributeGroupRef(element, parent);
            if (parent is XsSchema || (parent == null && element.Parent.IfNotNull(p => p.Name) == Xs.Schema))
                return new XsAttributeGroup(element, parent);
        }
        if (element.Name == Xs.Choice) return new XsChoice(element, parent);
        if (element.Name == Xs.ComplexContent) return new XsComplexContent(element, parent);
        if (element.Name == Xs.ComplexType) return new XsComplexType(element, parent);
        //if (element.Name == Xs.DefaultOpenContent) return new DefaultOpenContent(element, parent);
        if (element.Name == Xs.Documentation) return new XsDocumentation(element, parent);
        if (element.Name == Xs.Element) return new XsElement(element, parent);
        if (element.Name == Xs.Enumeration) return XsFacet.Create(element, parent);
        if (element.Name == Xs.Extension)
        {
            if (parent is XsComplexContent || (parent == null && element.Parent.IfNotNull(p => p.Name) == Xs.ComplexContent)) 
                return new XsComplexContentExtension(element, parent);
            if (parent is XsSimpleContent || (parent == null && element.Parent.IfNotNull(p => p.Name) == Xs.SimpleContent)) 
                return new XsSimpleContentExtension(element, parent);
        }
        //if (element.Name == Xs.Field) return new Field(element, parent);
        if (element.Name == Xs.Group)
        {
            if (parent is XsSchema || (parent == null && element.Parent.IfNotNull(p => p.Name) == Xs.Schema)) 
                return new XsGroup(element, parent);
            if (parent is XsChoice || parent is XsComplexType || parent is XsSequence ||
                (parent == null && element.Parent.IfNotNull(p => new [] {Xs.Choice, Xs.ComplexType, Xs.Sequence}.Contains(p.Name)))) 
                return new XsGroupRef(element, parent);
        }
        if (element.Name == Xs.Import) return new XsImport(element, parent);
        if (element.Name == Xs.Include) return new XsInclude(element, parent);
        //if (element.Name == Xs.Key) return new Key(element, parent);
        //if (element.Name == Xs.Keyref) return new Keyref(element, parent);
        if (element.Name == Xs.List) return new XsSimpleTypeList(element, parent);
        //if (element.Name == Xs.MaxScale) return new MaxScale(element, parent);
        //if (element.Name == Xs.MinScale) return new MinScale(element, parent);
        if (element.Name == Xs.Notation) return new XsNotation(element, parent);
        //if (element.Name == Xs.OpenContent) return new OpenContent(element, parent);
        //if (element.Name == Xs.Override) return new Override(element, parent);
        if (element.Name == Xs.Redefine) return new XsRedefine(element, parent);
        if (element.Name == Xs.Restriction)
        {
            if (parent is XsComplexContent || (parent == null && element.Parent.IfNotNull(p => p.Name) == Xs.ComplexContent))
                return new XsComplexContentRestriction(element, parent);
            if (parent is XsSimpleContent || (parent == null && element.Parent.IfNotNull(p => p.Name) == Xs.SimpleContent)) 
                return new XsSimpleContentRestriction(element, parent);
            if (parent is XsSimpleType || (parent == null && element.Parent.IfNotNull(p => p.Name) == Xs.SimpleType)) 
                return new XsSimpleTypeRestriction(element, parent);
        }
        // Omitted - Use XsSchema.Load.. if (element.Name == Xs.Schema) return new XsSchema(element);
        //if (element.Name == Xs.Selector) return new Selector(element, parent);
        if (element.Name == Xs.Sequence) return new XsSequence(element, parent);
        if (element.Name == Xs.SimpleContent) return new XsSimpleContent(element, parent);
        if (element.Name == Xs.SimpleType) return new XsSimpleType(element, parent);
        if (element.Name == Xs.Union) return new XsSimpleTypeUnion(element, parent);
        //if (element.Name == Xs.Unique) return new Unique(element, parent);
        
        throw new ArgumentException(String.Format(CultureInfo.InvariantCulture,
                                                  "Unsupported schema element: {0}", element.Name));
    }

    private static readonly List<Tuple<XName, Type>> _schemaTypesNamePairs =
        new List<Tuple<XName, Type>>
            {
                Tuple.Create(Xs.All, typeof (XsAll)),
                //Tuple.Create(Xs.Alternative, typeof(Alternative)),
                Tuple.Create(Xs.Annotation, typeof (XsAnnotation)),
                Tuple.Create(Xs.Any, typeof (XsAny)),
                Tuple.Create(Xs.AnyAttribute, typeof (XsAnyAttribute)),
                Tuple.Create(Xs.AppInfo, typeof (XsAppInfo)),
                //Tuple.Create(Xs.Assert, typeof(Assert)),
                //Tuple.Create(Xs.Assertion, typeof(Assertion)),
                Tuple.Create(Xs.Attribute, typeof (Attribute)),
                Tuple.Create(Xs.AttributeGroup, typeof(XsAttributeGroup)),
                Tuple.Create(Xs.Choice, typeof (XsChoice)),
                Tuple.Create(Xs.ComplexContent, typeof(XsComplexContent)),
                Tuple.Create(Xs.ComplexType, typeof (XsComplexType)),
                //Tuple.Create(Xs.DefaultOpenContent, typeof(DefaultOpenContent) ),
                Tuple.Create(Xs.Documentation, typeof (XsDocumentation)),
                Tuple.Create(Xs.Element, typeof (XsElement)),
                Tuple.Create(Xs.Enumeration, typeof(XsFacet)),
                Tuple.Create(Xs.ExplicitTimezone, typeof(XsFacet)),
                Tuple.Create(Xs.Extension, typeof(XsComplexContentExtension) ),
                Tuple.Create(Xs.Extension, typeof(XsSimpleContentExtension) ),
                //Tuple.Create(Xs.Field, typeof(Field) ),
                Tuple.Create(Xs.FractionDigits, typeof(XsFacet)),
                Tuple.Create(Xs.Group, typeof(XsGroup)),
                Tuple.Create(Xs.Import, typeof(XsImport) ),
                Tuple.Create(Xs.Include, typeof(XsInclude) ),
                //Tuple.Create(Xs.Key, typeof(Key) ),
                //Tuple.Create(Xs.Keyref, typeof(Keyref) ),
                Tuple.Create(Xs.Length, typeof(XsFacet)),
                Tuple.Create(Xs.List, typeof(XsSimpleTypeList)),
                Tuple.Create(Xs.MaxExclusive, typeof(XsFacet) ),
                Tuple.Create(Xs.MaxInclusive, typeof(XsFacet)),
                Tuple.Create(Xs.MaxLength, typeof(XsFacet)),
                //Tuple.Create(Xs.MaxScale, typeof(MaxScale) ),
                Tuple.Create(Xs.MinExclusive, typeof(XsFacet)),
                Tuple.Create(Xs.MinInclusive, typeof(XsFacet)),
                Tuple.Create(Xs.MinLength, typeof(XsFacet)),
                //Tuple.Create(Xs.MinScale, typeof(MinScale) ),
                Tuple.Create(Xs.Notation, typeof(XsNotation)),
                //Tuple.Create(Xs.OpenContent, typeof(OpenContent) ),
                //Tuple.Create(Xs.Override, typeof(Override) ),
                Tuple.Create(Xs.Pattern, typeof(XsFacet)),
                Tuple.Create(Xs.Redefine, typeof(XsRedefine) ),
                Tuple.Create(Xs.Restriction, typeof(XsComplexContentRestriction)),
                Tuple.Create(Xs.Restriction, typeof(XsSimpleContentRestriction)),
                Tuple.Create(Xs.Restriction, typeof(XsSimpleTypeRestriction)),
                Tuple.Create(Xs.Schema, typeof (XsSchema)),
                //Tuple.Create(Xs.Selector, typeof(Selector) ),
                Tuple.Create(Xs.Sequence, typeof (XsSequence)),
                Tuple.Create(Xs.SimpleContent, typeof(XsSimpleContent)),
                Tuple.Create(Xs.SimpleType, typeof (XsSimpleType)),
                Tuple.Create(Xs.TotalDigits, typeof(XsFacet)),
                Tuple.Create(Xs.Union, typeof(XsSimpleTypeUnion)),
                //Tuple.Create(Xs.Unique, typeof(Unique)),
                Tuple.Create(Xs.WhiteSpace, typeof(XsFacet))
            };

    private static readonly Lazy<ILookup<Type, XName>> _schemaNamesByType =
        new(() => _schemaTypesNamePairs.LookupFromItem1());

    private static readonly Lazy<ILookup<XName, Type>> _schemaTypesByName =
        new(() => _schemaTypesNamePairs.LookupFromItem2());

    public static ILookup<Type, XName> SchemaNamesByType
    {
        get { return _schemaNamesByType.Value; }
    }

    public static ILookup<XName, Type> SchemaTypesByName
    {
        get { return _schemaTypesByName.Value; }
    }

    /// <exception cref="ArgumentException"><c>ArgumentException</c>.</exception>
    private static string getNameForType(Type schemaObjectType)
    {
        var names = SchemaNamesByType[schemaObjectType];
        int count = names.Count();
        if (count == 0)
            throw new ArgumentException(String.Format(CultureInfo.InvariantCulture,
                                                      "Unsupported schema object type: {0}",
                                                      schemaObjectType.FullName));

        if (count == 1) return names.Single().ToString();
        return String.Join(",", names.Select(name => name.ToString()).ToArray());
    }
}
