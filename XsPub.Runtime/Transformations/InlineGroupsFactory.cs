using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using XsPub.Runtime.Properties;
using XsPub.Runtime.Settings;

namespace XsPub.Runtime.Transformations
{
    /// <summary>
    /// A transform which replaces <xs:group ref="name"> with the contents of <xs:group name="name">.
    /// </summary>
    /// <remarks>
    /// This transform is useful for many tools which do not support group references.  The output schema is no more loose or strict than the original schema, but is simpler.
    /// 
    /// This allows groups to be used in the source schema for maintainability while being able to maintain broader compatability.
    /// </remarks>
    [Export(typeof(ITransformationFactory))]
    internal class InlineGroupsFactory : TransformationFactoryBase
    {
        private const string TransformationName = "InlineGroups";

        public override bool IsSingleton
        {
            get { return true; }
        }

        public override string Name
        {
            get { return TransformationName; }
        }

        public override string DisplayName
        {
            get { return Resources.InlineGroupsDisplayName; }
        }

        public override string Description
        {
            get { return Resources.InlineGroupsDescription; }
        }

        public override IEnumerable<ITransformation> CreateTransformations(RuntimeSettingSet settings)
        {
            return settings.SetsByTransformation[TransformationName].Select(setting => new InlineGroups());
        }
    }
}