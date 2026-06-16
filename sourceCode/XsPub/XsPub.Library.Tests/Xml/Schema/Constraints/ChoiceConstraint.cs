using System;
using System.Xml.Schema;
using XsPub.Library.Xml.Schema;

namespace XsPub.Library.Tests.Xml.Schema.Constraints
{
    public class ChoiceConstraint : GroupBaseConstraint<XsChoice, XmlSchemaChoice>
    {
        public override string GetActualName(XsChoice actual)
        {
            return null;
        }

        public override string GetExpectedName(XmlSchemaChoice expected)
        {
            return null;
        }
    }
}