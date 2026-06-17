using System.Xml.Linq;

namespace XsPub.Library.Xml.Schema
{
    interface IGlobalNamedObject : INamedObject
    {
        XName QualifiedName { get; }
    }
}