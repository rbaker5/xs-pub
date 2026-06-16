using System;
using System.Xml.Schema;
using XsPub.Library.Xml.Schema;

namespace XsPub.Library.Tests.Xml.Schema.Constraints
{
    public class AllConstraint : GroupBaseConstraint<XsAll, XmlSchemaAll>
    {
        public override string GetActualName(XsAll actual)
        {
            return null;
        }

        public override string GetExpectedName(XmlSchemaAll expected)
        {
            return null;
        }
    }
}