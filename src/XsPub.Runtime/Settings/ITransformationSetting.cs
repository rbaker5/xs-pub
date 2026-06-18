namespace XsPub.Runtime.Settings
{
    public interface ITransformationSetting
    {
        bool ValueIsSet { get; }
        object? CurrentValue { get; }
        string Name { get; }
        string DisplayName { get; }
        string Description { get; }
        void Reset();
        ITransformationSetting CloneAsDefault();
    }
}
