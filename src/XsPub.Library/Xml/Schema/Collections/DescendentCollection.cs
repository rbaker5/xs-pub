using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Xml.Linq;

namespace XsPub.Library.Xml.Schema.Collections;

public class DescendentCollection<TItem> : IEnumerable<TItem> 
    where TItem : XsObject
{
    public DescendentCollection(XsObject parent)
    {
        Parent = parent;
    }

    protected XsObject Parent { get; private set; }

    public IEnumerable<TOutput> OfType<TOutput>() where TOutput : XsObject
    {
        var returnType = typeof(TOutput);
        return OfType(returnType).Cast<TOutput>();
    }

    public IEnumerable<XsObject> OfType(Type returnType)
    {
        if (returnType == typeof(XsGroup))
        {
            var redefine = Parent as XsRedefine;
            if (redefine != null) return redefine.Groups;
            
            var schema = Parent as XsSchema;
            var external = Parent as XsExternal;
            if (external != null) schema = external.Schema;
            if (schema != null) return schema.Groups;
        }
        if (returnType == typeof(XsGroupRef))
        {
            return
                Parent.Element.Descendants(Xs.Group).Attributes(XsA.Ref).Select(attribute => attribute.Parent)
                    .Select(CreateDeepSchemaObject);
        }
        // If the type is conditional based upon placement in the hierarchy, a custom rule like above is necessary.
        var names = XsObjectFactory.SchemaNamesByType[returnType];
        if (names.Count() == 1)
        {
            return Parent.Element.Descendants(names.Single()).Select(CreateDeepSchemaObject);
        }
        throw new NotImplementedException();
    }

    protected XsObject CreateSchemaObject(XElement element) 
    {
        return XsObjectFactory.Create<XsObject>(element, Parent);
    }

    /// <summary>
    /// Creates schema objects that the parent/child chain ActualHas not been resolved for.
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    protected XsObject CreateDeepSchemaObject(XElement? element)
    {
        return XsObjectFactory.Create<XsObject>(element);
    }

    public IEnumerator<TItem> GetEnumerator()
    {
        return Parent.Element.Descendants().Select(CreateDeepSchemaObject).Cast<TItem>().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
