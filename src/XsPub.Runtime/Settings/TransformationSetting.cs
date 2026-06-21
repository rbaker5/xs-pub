using System.Collections.Generic;

namespace XsPub.Runtime.Settings;

public class TransformationSetting<T> : ITransformationSetting
{
    private T _currentValue;
    public T CurrentValue
    {
        get { return ValueIsSet ? _currentValue : Default; }
        set
        {
            ValueIsSet = true;
            _currentValue = value;
        }
    }

    object ITransformationSetting.CurrentValue { get { return CurrentValue; } }

    public bool ValueIsSet { get; private set; }
    public T Default { get; private set; }
    public string Name { get; private set; }
    public string DisplayName { get; private set; }
    public string Description { get; private set; }
    public void Reset()
    {
        ValueIsSet = false;
    }

    internal TransformationSetting(T defaultValue, string name, string displayName, string description)
    {
        Default = defaultValue;
        Name = name;
        DisplayName = displayName;
        Description = description;
    }

    public ITransformationSetting CloneAsDefault()
    {
        return new TransformationSetting<T>(Default, Name, DisplayName, Description);
    }
}

public static class TransformationSetting
{
    public static TransformationSetting<T> Create<T>(T defaultValue, string name, string displayName, string description)
    {
        return new TransformationSetting<T>(defaultValue, name, displayName, description);
    }

    public static void Add<T>(this List<ITransformationSetting> settings, T defaultValue, string name, string displayName, string description)
    {
        settings.Add(Create(defaultValue, name, displayName, description));
    }
}