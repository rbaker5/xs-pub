using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using XsPub.Runtime.Properties;
using XsPub.Runtime.Settings;

namespace XsPub.Runtime.Transformations
{
    [Export(typeof(ITransformationFactory))]
    internal class AllToSequenceFactory : TransformationFactoryBase
    {
        private const string TransformationName = "AllToSequence";

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
            get { return Resources.AllToSequenceDisplayName; }
        }

        public override string Description
        {
            get { return Resources.AllToSequenceDescription; }
        }

        public override IEnumerable<ITransformation> CreateTransformations(RuntimeSettingSet settings)
        {
            return settings.SetsByTransformation[TransformationName].Select(setting => new AllToSequence());
        }
    }
}