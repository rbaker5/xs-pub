using XsPub.Runtime.Settings;

namespace XsPub.Runtime.Transformations;

public class ExplicitFormsFactory : TransformationFactoryBase
{
    private const string TransformationName = "ExplicitForms";

    public override bool IsSingleton => true;
    public override string Name => TransformationName;
    public override string DisplayName => "Explicit Forms";
    public override string Description => "Ensures attributeFormDefault and elementFormDefault are explicitly stated in the schema.";

    public override IEnumerable<ITransformation> CreateTransformations(RuntimeSettingSet settings)
    {
        return settings.SetsByTransformation[TransformationName].Select(_ => (ITransformation)new ExplicitForms());
    }
}
