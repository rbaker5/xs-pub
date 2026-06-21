using XsPub.Runtime;
using XsPub.Runtime.Settings;
using Xunit;

namespace XsPub.Tests;

// ── TransformationSetting<T> ──────────────────────────────────────────────

public class TransformationSettingTests
{
    private static TransformationSetting<T> Make<T>(T defaultValue) =>
        TransformationSetting.Create(defaultValue, "Prop", "Property", "A property.");

    [Fact]
    public void DefaultValue_ReturnedBeforeSet()
    {
        var s = Make("hello");
        Assert.Equal("hello", s.CurrentValue);
        Assert.False(s.ValueIsSet);
    }

    [Fact]
    public void CurrentValue_AfterAssign_ReturnsNewValue()
    {
        var s = Make("hello");
        s.CurrentValue = "world";
        Assert.Equal("world", s.CurrentValue);
        Assert.True(s.ValueIsSet);
    }

    [Fact]
    public void Reset_ClearsValueIsSet_AndRestoresDefault()
    {
        var s = Make(42);
        s.CurrentValue = 99;
        s.Reset();
        Assert.False(s.ValueIsSet);
        Assert.Equal(42, s.CurrentValue);
    }

    [Fact]
    public void CloneAsDefault_HasSameDefault_ButIndependentState()
    {
        var s = Make("orig");
        s.CurrentValue = "changed";

        var clone = (TransformationSetting<string>)s.CloneAsDefault();
        Assert.Equal("orig", clone.Default);
        Assert.Equal("orig", clone.CurrentValue);
        Assert.False(clone.ValueIsSet);
    }

    [Fact]
    public void Metadata_Preserved()
    {
        var s = Make(true);
        Assert.Equal("Prop", s.Name);
        Assert.Equal("Property", s.DisplayName);
        Assert.Equal("A property.", s.Description);
    }

    [Fact]
    public void Bool_Setting_RoundTrip()
    {
        var s = Make(false);
        Assert.False(s.CurrentValue);
        s.CurrentValue = true;
        Assert.True(s.CurrentValue);
    }
}

// ── SettingSet ────────────────────────────────────────────────────────────

public class SettingSetTests
{
    private static SettingSet MakeSet() =>
        new SettingSet("MyTransform", "My Transformation", "Does something.",
            new List<ITransformationSetting>
            {
                TransformationSetting.Create(10, "Count", "Count", "How many."),
                TransformationSetting.Create("default", "Label", "Label", "A label."),
            });

    [Fact]
    public void GetPropertyValue_ReturnsDefault()
    {
        var s = MakeSet();
        Assert.Equal(10, s.GetPropertyValue<int>("Count"));
        Assert.Equal("default", s.GetPropertyValue<string>("Label"));
    }

    [Fact]
    public void SetPropertyValue_ChangesCurrentValue()
    {
        var s = MakeSet();
        s.SetPropertyValue("Count", 99);
        Assert.Equal(99, s.GetPropertyValue<int>("Count"));
    }

    [Fact]
    public void SetPropertyValueWithString_ConvertsType()
    {
        var s = MakeSet();
        s.SetPropertyValueWithString("Count", "7");
        Assert.Equal(7, s.GetPropertyValue<int>("Count"));
    }

    [Fact]
    public void SetPropertyValue_UnknownName_Throws()
    {
        var s = MakeSet();
        Assert.Throws<ArgumentException>(() => s.SetPropertyValue("Nonexistent", 0));
    }

    [Fact]
    public void GetPropertyValue_UnknownName_Throws()
    {
        var s = MakeSet();
        Assert.Throws<ArgumentException>(() => s.GetPropertyValue<int>("Nonexistent"));
    }

    [Fact]
    public void IsValidSetting_TrueForKnownName()
    {
        var s = MakeSet();
        Assert.True(s.IsValidSetting("Count"));
        Assert.True(s.IsValidSetting("Label"));
        Assert.False(s.IsValidSetting("Nope"));
    }

    [Fact]
    public void Reset_RestoresDefaults()
    {
        var s = MakeSet();
        s.SetPropertyValue("Count", 77);
        s.Reset();
        Assert.Equal(10, s.GetPropertyValue<int>("Count"));
    }

    [Fact]
    public void Count_MatchesNumberOfSettings()
    {
        var s = MakeSet();
        Assert.Equal(2, s.Count);
    }

    [Fact]
    public void ValuesToXElement_OnlyIncludesSetValues()
    {
        var s = MakeSet();
        s.SetPropertyValue("Count", 5);
        var el = s.ValuesToXElement();
        Assert.Single(el.Elements());
        Assert.Equal("Count", el.Elements().First().Name.LocalName);
    }

    [Fact]
    public void ReadValuesFrom_LoadsFromElement()
    {
        var s = MakeSet();
        s.SetPropertyValue("Count", 3);
        var el = s.ValuesToXElement();

        var fresh = MakeSet();
        fresh.ReadValuesFrom(el);
        Assert.Equal(3, fresh.GetPropertyValue<int>("Count"));
    }

    [Fact]
    public void Name_DefaultsToTransformationName()
    {
        var s = new SettingSet("Transform", "Display", "Desc");
        Assert.Equal("Transform", s.Name);
        Assert.Equal("Transform", s.TransformationName);
    }

    [Fact]
    public void Name_CanBeOverridden()
    {
        var s = new SettingSet("Transform", "Display", "Desc", "InstanceName",
            Enumerable.Empty<ITransformationSetting>());
        Assert.Equal("InstanceName", s.Name);
        Assert.Equal("Transform", s.TransformationName);
    }
}

// ── RuntimeSettingSet ─────────────────────────────────────────────────────

public class RuntimeSettingSetTests
{
    private static RuntimeSettingSet MakeRuntime() =>
        new RuntimeSettingSet(
            new SettingSet("Global", "Global Settings", "Global."),
            Enumerable.Empty<SettingSet>());

    private static SettingSet MakeTransformSet(string name) =>
        new SettingSet(name, $"{name} display", $"{name} desc.");

    [Fact]
    public void GlobalSettings_AccessibleAfterConstruction()
    {
        var r = MakeRuntime();
        Assert.Equal("Global", r.GlobalSettings.Name);
    }

    [Fact]
    public void Add_AppearsInSetsByName()
    {
        var r = MakeRuntime();
        var ts = MakeTransformSet("MyT");
        r.Add(ts);
        Assert.True(r.SetsByName.ContainsKey("MyT"));
    }

    [Fact]
    public void Add_DuplicateName_Throws()
    {
        var r = MakeRuntime();
        r.Add(MakeTransformSet("T1"));
        Assert.Throws<ArgumentException>(() => r.Add(MakeTransformSet("T1")));
    }

    [Fact]
    public void AddRange_AddsMultiple()
    {
        var r = MakeRuntime();
        r.AddRange(new[] { MakeTransformSet("A"), MakeTransformSet("B") });
        Assert.True(r.SetsByName.ContainsKey("A"));
        Assert.True(r.SetsByName.ContainsKey("B"));
    }

    [Fact]
    public void Remove_ByObject_RemovesFromLookup()
    {
        var r = MakeRuntime();
        var ts = MakeTransformSet("T");
        r.Add(ts);
        Assert.True(r.Remove(ts));
        Assert.False(r.SetsByName.ContainsKey("T"));
    }

    [Fact]
    public void Remove_ByName_RemovesFromLookup()
    {
        var r = MakeRuntime();
        r.Add(MakeTransformSet("T"));
        Assert.True(r.Remove("T"));
        Assert.False(r.SetsByName.ContainsKey("T"));
    }

    [Fact]
    public void Remove_GlobalSettings_ReturnsFalse()
    {
        var r = MakeRuntime();
        Assert.False(r.Remove(r.GlobalSettings));
        Assert.False(r.Remove("Global"));
    }

    [Fact]
    public void RemoveAllForTransformation_ClearsAll()
    {
        var r = MakeRuntime();
        r.Add(new SettingSet("Trans", "T", "T.", "Inst1", Enumerable.Empty<ITransformationSetting>()));
        r.Add(new SettingSet("Trans", "T", "T.", "Inst2", Enumerable.Empty<ITransformationSetting>()));
        Assert.True(r.RemoveAllForTransformation("Trans"));
        Assert.False(r.SetsByName.ContainsKey("Inst1"));
        Assert.False(r.SetsByName.ContainsKey("Inst2"));
    }

    [Fact]
    public void SetsByTransformation_GroupsCorrectly()
    {
        var r = MakeRuntime();
        r.Add(new SettingSet("Trans", "T", "T.", "A", Enumerable.Empty<ITransformationSetting>()));
        r.Add(new SettingSet("Trans", "T", "T.", "B", Enumerable.Empty<ITransformationSetting>()));
        Assert.Equal(2, r.SetsByTransformation["Trans"].Count());
    }

    [Fact]
    public void Reset_RemovesAddedSets()
    {
        var r = MakeRuntime();
        r.Add(MakeTransformSet("X"));
        r.Reset();
        Assert.False(r.SetsByName.ContainsKey("X"));
        Assert.True(r.SetsByName.ContainsKey("Global"));
    }
}
