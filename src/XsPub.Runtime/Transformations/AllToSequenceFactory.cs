using XsPub.Runtime.Settings;

namespace XsPub.Runtime.Transformations;

public class AllToSequenceFactory : TransformationFactoryBase
{
    private const string TransformationName = "AllToSequence";

    public override bool IsSingleton => true;
    public override string Name => TransformationName;
    public override string DisplayName => "Convert xs:all to xs:sequence";
    public override string Description => "Replaces occurrences of xs:all with xs:sequence.";

    public override IEnumerable<ITransformation> CreateTransformations(RuntimeSettingSet settings)
    {
        return settings.SetsByTransformation[TransformationName].Select(_ => (ITransformation)new AllToSequence());
    }
}
