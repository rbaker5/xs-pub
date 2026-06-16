using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using XsPub.Runtime.Properties;
using XsPub.Runtime.Settings;

namespace XsPub.Runtime.Transformations
{
    [Export(typeof(ITransformationFactory))]
    internal class ExplicitFormsFactory : TransformationFactoryBase
    {
        private const string TransformationName = "ExplicitForms";

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
            get { return Resources.ExplicitFormsDisplayName; }
        }

        public override string Description
        {
            get { return Resources.ExplicitFormsDescription; }
        }

        public override IEnumerable<ITransformation> CreateTransformations(RuntimeSettingSet settings)
        {
            return settings.SetsByTransformation[TransformationName].Select(setting => new ExplicitForms());
        }
    }
}