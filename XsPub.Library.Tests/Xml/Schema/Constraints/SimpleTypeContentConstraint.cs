using System;
using System.Xml.Schema;
using XsPub.Library.Xml.Schema;

namespace XsPub.Library.Tests.Xml.Schema.Constraints
{
    public abstract class SimpleTypeContentConstraint<TActual, TExpected> : AnnotatedConstraint<TActual, TExpected>
        where TActual : XsSimpleTypeContent
        where TExpected : XmlSchemaSimpleTypeContent
    {
    }
}