using XsPub.Runtime.Settings;

namespace XsPub.Runtime.Transformations
{
    public class InlineGroupsFactory : TransformationFactoryBase
    {
        private const string TransformationName = "InlineGroups";

        public override bool IsSingleton => true;
        public override string Name => TransformationName;
        public override string DisplayName => "Inline Groups";
        public override string Description => "Replaces group references with the referenced content.";

        public override IEnumerable<ITransformation> CreateTransformations(RuntimeSettingSet settings)
        {
            return settings.SetsByTransformation[TransformationName].Select(_ => (ITransformation)new InlineGroups());
        }
    }
}
