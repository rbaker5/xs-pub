using System.Collections.Generic;
using System.Xml.Linq;

namespace XsPub.Library.Xml.Schema;

public interface INamedCollection<T> : ICollection<T>
{
    T this[string name] { get; }
}

public interface IGlobalNamedCollection<T> : IEnumerable<T>
{
    T this[XName name] { get; }
}