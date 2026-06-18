using System.Xml.Linq;
using XsPub.Library.Xml.Schema;
using XsPub.Runtime.Settings;

namespace XsPub.Runtime.Transformations
{
    // ExplicitValue<T> is incomplete (selector never wired up in original source). Preserved as-is.
    public class ExplicitValue<T> : ITransformation
    {
        public List<XName>? ElementNames;
        public XName? AttributeName;
        public Func<XsObject, bool>? Selector;
        public string? DefaultValue;
        public Type? ObjectType;

        public void GatherData(XsSchema schema) { }

        public void IndependentTransform(XsSchema schema)
        {
            if (Selector == null || AttributeName == null || DefaultValue == null)
                return;
            var elements = schema.Descendents.OfType(typeof(T)).Where(o => Selector((XsObject)o));
            elements.AsParallel().ForAll(obj => ((XsObject)obj).SetAttributeValue(AttributeName, DefaultValue));
        }

        public bool DependentTransform(XsSchema schema) => false;
    }
}
