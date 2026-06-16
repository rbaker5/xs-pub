using System;
using System.Linq;
using System.Xml.Schema;
using NUnit.Framework;
using XsPub.Library.Xml.Schema;

namespace XsPub.Library.Tests.Xml.Schema.Constraints
{
    public class ComplexTypeConstraint : TypeConstraint<XsComplexType, XmlSchemaComplexType>
    {
        protected override void AddCustomConstraints()
        {
            base.AddCustomConstraints();

            //Add(ActualHas.Property("AttributeUses").EqualTo(Expected.AttributeUses));
            //Add(ActualHas.Property("AttributeWildcard").EqualTo(Expected.AttributeWildcard));

            Add(ActualHas.Property("Block").EqualTo(Expected.Block));
            Add(ActualHas.Property("BlockResolved").EqualTo(Expected.BlockResolved));
            Add(ActualHas.Property("ContentType").EqualTo(Expected.ContentType));
            Add(ActualHas.Property("IsAbstract").EqualTo(Expected.IsAbstract));

            Add(ActualHas.Property("AnyAttribute").ComparableTo(Expected.AnyAttribute));
            Add(ActualHas.Property("ContentModel").ComparableTo(Expected.ContentModel));
            //Add(ActualHas.Property("ContentTypeParticle").ComparableTo(Expected.ContentTypeParticle));
            Add(ActualHas.Property("Particle").ComparableTo(Expected.Particle));
            Add(ActualHas.Property("Attributes").ComparableTo(Expected.Attributes.Cast<XmlSchemaObject>()));
        }
    }
}