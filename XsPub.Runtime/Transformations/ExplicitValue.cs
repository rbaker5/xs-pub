using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Xml.Linq;
using XsPub.Library.Xml.Schema;
using XsPub.Runtime.Properties;
using XsPub.Runtime.Settings;

namespace XsPub.Runtime.Transformations
{
    [Export(typeof(ITransformationFactory))]
    internal class ExplicitValueFactory : TransformationFactoryBase
    {
        private const string TransformationName = "ExplicitValue";

        private List<ITransformationSetting> _defaults =
            new List<ITransformationSetting>
                {
                    TransformationSetting.Create(typeof(XsObject), "ObjectType", "Object Type", "")
                };

        public override bool IsSingleton
        {
            get { return false; }
        }

        public override string Name
        {
            get { return TransformationName; }
        }

        public override string DisplayName
        {
            get { return Resources.ExplicitValueDisplayName; }
        }

        public override string Description
        {
            get { return Resources.ExplicitValueDescription; }
        }

        public override IEnumerable<ITransformation> CreateTransformations(RuntimeSettingSet settings)
        {
            return settings.SetsByTransformation[TransformationName].Select(createTransformation);
        }

        private ITransformation createTransformation(SettingSet set)
        {
            Type objectType = set.GetPropertyValue<Type>("ObjectType");
            return typeof (ExplicitValue<object>).GetGenericTypeDefinition().MakeGenericType(objectType).GetConstructor(null) as ITransformation;
        }

        public override SettingSet GetDefaultSettings()
        {
            return new SettingSet(Name, DisplayName, Description, Guid.NewGuid().ToString(), _defaults);
        }
    }

    public class ExplicitValue<T> : ITransformation
    {
        public List<XName> ElementNames;
        public XName AttributeName;
        public Func<XsObject, bool> _selector;
        public string DefaultValue;

        public Type ObjectType;

        //protected override IEnumerable<XElement> GetInvalidElements()
        //{
        //    return
        //        ElementNames.SelectMany(name => Container.Descendants(name)).Where(
        //            schema => schema.Attribute(AttributeName) == null);
        //}

        //protected override void TransformInternal(XElement element)
        //{
        //    element.SetAttributeValue(AttributeName, DefaultValue);
        //}

        #region ITransformation Members

        public void GatherData(XsSchema schema)
        {
        }

        public void IndependentTransform(XsSchema schema)
        {
            var elements = schema.Descendents.OfType(ObjectType).Where(_selector);
            elements.AsParallel().ForAll(obj => obj.SetAttributeValue(AttributeName, DefaultValue));
        }

        public bool DependentTransform(XsSchema schema)
        {
            return false;
        }
        #endregion
    }
}