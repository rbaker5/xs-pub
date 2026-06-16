using System;
using System.Xml.Schema;
using NUnit.Framework;
using XsPub.Library.Xml.Schema;

namespace XsPub.Library.Tests.Xml.Schema.Constraints
{
    public class GroupRefConstraint : ParticleConstraint<XsGroupRef, XmlSchemaGroupRef>
    {
        protected override void AddCustomConstraints()
        {
            base.AddCustomConstraints();
            Add(ActualHas.Property("RefName").ComparableTo(Expected.RefName));
            //Add(ActualHas.Property("Particle").ComparableTo(Expected.Particle));
        }

        public override string GetActualName(XsGroupRef actual)
        {
            return String.Format("{{{0}}}{1}", actual.RefName.NamespaceName, actual.RefName.LocalName);
        }

        public override string GetExpectedName(XmlSchemaGroupRef expected)
        {
            return String.Format("{{{0}}}{1}", expected.RefName.Namespace, expected.RefName.Name);
        }
    }
}