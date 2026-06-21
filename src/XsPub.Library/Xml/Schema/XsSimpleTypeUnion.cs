using System.Collections.Generic;
using System.Xml.Linq;
using XsPub.Library.Xml.Schema.Collections;

namespace XsPub.Library.Xml.Schema;

public class XsSimpleTypeUnion : XsSimpleTypeContent
{
    public XsSimpleTypeUnion(XElement element)
        : base(element)
    {
    }

    public XsSimpleTypeUnion(XElement element, XsObject parent)
        : base(element, parent)
    {
    }


    // public IEnumerable<XsSimpleType> BaseMemberTypes {get;}

    private ICollection<XsSimpleType> _baseTypes;
    public ICollection<XsSimpleType> BaseTypes
    {
        get
        {
            if (_baseTypes == null) _baseTypes = XsCollection.Create<XsSimpleType>(this, Xs.SimpleType);
            return _baseTypes;
        }
    }

    // public ICollection<XmlQualifiedName> MemberTypes {get; set;}
}