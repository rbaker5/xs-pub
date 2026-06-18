using System.Xml.Schema;
using XsPub.Library.Xml.Schema;

namespace XsPub.Library.Tests.Xml.Schema.Constraints;

public class SequenceConstraint : GroupBaseConstraint<XsSequence, XmlSchemaSequence>
{
    public override string? GetActualName(XsSequence actual) => null;
    public override string? GetExpectedName(XmlSchemaSequence expected) => null;
}
