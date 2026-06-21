using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace XsPub.Library.Xml.Schema;

public class XsGroup : XsAnnotated, IGlobalNamedObject
{
    public XsGroup(XElement element)
        : base(element)
    {
    }

    public XsGroup(XElement element, XsObject parent)
        : base(element, parent)
    {
    }

    #region Name attributes
    /// <summary>
    /// Gets or sets the name of the schema group.
    /// </summary>
    /// <value>
    /// The name of the schema group. 
    /// </value>
    public string Name
    {
        get { return GetAttributeValueInternal(XsA.Name); }
        set { SetAttributeValueInternal(XsA.Name, value); }
    }

    public XName QualifiedName { get { return GetGlobalQualifiedName(XsA.Name); } }
    #endregion

    private static readonly IEnumerable<XName> _particleElementNames = new[] { Xs.Sequence, Xs.Choice, Xs.All };
    public XsParticle Particle
    {
        get { return GetExclusiveOrElement<XsParticle>(_particleElementNames); }
        set { SetExclusiveOrElement(_particleElementNames, value); }
    }

    protected override bool IsValidAttribute(XName attributeName)
    {
        if (attributeName == XsA.Name) return true;
        return base.IsValidAttribute(attributeName);
    }
}