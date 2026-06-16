using System;
using System.Xml.Schema;
using NUnit.Framework;
using XsPub.Library.Xml.Schema;

namespace XsPub.Library.Tests.Xml.Schema.Constraints
{
    public abstract class TypeConstraint<TActual, TExpected> : AnnotatedConstraint<TActual, TExpected>
        where TActual : XsType
        where TExpected : XmlSchemaType
    {
        protected override void AddCustomConstraints()
        {
            //Add(ActualHas.Property("DerivedBy").EqualTo(Expected.DerivedBy));
            Add(ActualHas.Property("Final").EqualTo(Expected.Final));
            Add(ActualHas.Property("FinalResolved").EqualTo(Expected.FinalResolved));
            Add(ActualHas.Property("IsMixed").EqualTo(Expected.IsMixed));
            Add(ActualHas.Property("Name").EqualTo(Expected.Name));
            //Add(ActualHas.Property("TypeCode").EqualTo(Expected.TypeCode));
            
            Add(ActualHas.Property("QualifiedName").ComparableTo(Expected.QualifiedName));
            //Add(ActualHas.Property("BaseXmlSchemaType").ComparableTo(Expected.BaseXmlSchemaType));
            //Add(ActualHas.Property("Datatype").ComparableTo(Expected.Datatype));
        }

        public override string GetActualName(TActual actual)
        {
            return actual.Name;
        }

        public override string GetExpectedName(TExpected expected)
        {
            return expected.Name;
        }
    }
}