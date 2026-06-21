using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml;
using System.Xml.Linq;

namespace XsPub.Library.Xml.Schema;

public class NamespaceCollection : IEnumerable<XmlQualifiedName>
{
    public XsObject Obj { get; private set; }
    public event EventHandler<XObjectChangeEventArgs> Changed;
    private Lazy<Dictionary<string, XmlQualifiedName>> _namespaces;

    public NamespaceCollection(XsObject obj)
    {
        Obj = obj;
        Obj.Element.Changed += nodeChanged;
        //if (Obj.Parent != null)
        //    Obj.Parent.Element.Changed += parentChanged;

        resetNamespaces();
    }

    private void nodeChanged(object sender, XObjectChangeEventArgs e)
    {
        var attribute = sender as XAttribute;
        // Skip descendents and non-attributes.
        if (attribute == null || attribute.Parent != Obj.Element) return;
        if (attribute.IsNamespaceDeclaration) resetNamespaces();
    }

    //private void parentChanged(object sender, XObjectChangeEventArgs e)
    //{
    //    var attribute = sender as XAttribute;
    //}


    private void resetNamespaces()
    {
        _namespaces = new Lazy<Dictionary<string, XmlQualifiedName>>(getNamespaces, LazyThreadSafetyMode.None);
    }

    public void Add(string prefix, string ns)
    {
        var attributeName = string.IsNullOrEmpty(prefix) || prefix == XNamespace.Xmlns.NamespaceName
                                ? XName.Get(XNamespace.Xmlns.NamespaceName)
                                : XNamespace.Xmlns.GetName(prefix);
        var attribute = Obj.Element.Attribute(attributeName);
        if (attribute == null)
            Obj.Element.Add(new XAttribute(attributeName, ns));
        else
            attribute.Value = ns;
    }

    private Dictionary<string, XmlQualifiedName> getNamespaces()
    {
        var namespaces = new Dictionary<string, XmlQualifiedName>();
        foreach (var localNamespace in Obj.Element.Attributes().Where(attribute => attribute.IsNamespaceDeclaration))
        {
            var prefix = localNamespace.Name.Namespace == XNamespace.Xmlns
                             ? localNamespace.Name.LocalName
                             : string.Empty;

            namespaces.Add(prefix, new XmlQualifiedName(prefix, localNamespace.Value));
        }
        return namespaces;
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
    /// </returns>
    public IEnumerator<XmlQualifiedName> GetEnumerator()
    {
        return _namespaces.Value.Values.GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
    /// </returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}