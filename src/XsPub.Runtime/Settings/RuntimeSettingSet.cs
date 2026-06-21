using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace XsPub.Runtime.Settings;

public class RuntimeSettingSet
{
    public SettingSet GlobalSettings { get; private set; }
    private readonly List<SettingSet> _transformationSettings;

    public IEnumerable<SettingSet> AllSettings { get { return _transformationSettings; } }
    
    private Lazy<Dictionary<string, SettingSet>> _setsByName;
    private Lazy<ILookup<string, SettingSet>> _setsByTransformation;
    public Dictionary<string, SettingSet> SetsByName { get { return _setsByName.Value; } }
    public ILookup<string, SettingSet> SetsByTransformation { get { return _setsByTransformation.Value; }}

    public RuntimeSettingSet(SettingSet globalSettings, IEnumerable<SettingSet> transformationSettings)
    {
        GlobalSettings = globalSettings;
        _transformationSettings = Enumerable.Repeat(GlobalSettings, 1).Concat(transformationSettings).ToList();
        resetLookups();
    }

    private void resetLookups()
    {
        if (_setsByName == null || _setsByName.IsValueCreated)
            _setsByName = new(() => _transformationSettings.ToDictionary(setting => setting.Name));
        if (_setsByTransformation == null || _setsByTransformation.IsValueCreated)
            _setsByTransformation = new(() => _transformationSettings.ToLookup(setting => setting.TransformationName));
    }

    public void AddRange(IEnumerable<SettingSet> settingSets)
    {
        foreach (var settingSet in settingSets)
        {
            if (SetsByName.ContainsKey(settingSet.Name))
                throw new ArgumentException(string.Format("Transformation with the name {0} already exists.",
                                                          settingSet.Name));

            _transformationSettings.Add(settingSet);
        }
        resetLookups();
    }

    public void Add(SettingSet settingSet)
    {
        if (SetsByName.ContainsKey(settingSet.Name))
            throw new ArgumentException(string.Format("Transformation with the name {0} already exists.", settingSet.Name));

        _transformationSettings.Add(settingSet);
        resetLookups();
    }

    public bool Remove(SettingSet settingSet)
    {
        if (settingSet == GlobalSettings)
            return false;

        if (!_transformationSettings.Remove(settingSet))
            return false;

        resetLookups();
        return true;
    }

    public bool Remove(string instanceName)
    {
        if (instanceName == GlobalSettings.Name)
            return false;

        if (_transformationSettings.RemoveAll(setting => setting.Name == instanceName) == 0)
            return false;

        resetLookups();
        return true;
    }

    public bool RemoveAllForTransformation(string tranformationName)
    {
        if (tranformationName == GlobalSettings.TransformationName)
            return false;

        if (_transformationSettings.RemoveAll(setting => setting.TransformationName == tranformationName) == 0)
            return false;

        resetLookups();
        return true;
    }

    public void ReadValuesFrom(XElement element, PublishingRuntime runtime)
    {
        foreach (var child in element.Elements())
        {
            var setName = child.Attribute("Name").Value;
            
            SettingSet set;
            if (setName == GlobalSettings.TransformationName)
            {
                set = GlobalSettings;
            }
            else if (!SetsByName.TryGetValue(setName, out set))
            {
                var tranformationName = child.Name.LocalName;
                var factory = runtime.TransformationFactories.Where(f => f.Name == tranformationName).SingleOrDefault();
                if (factory == null)
                    throw new InvalidOperationException(
                        string.Format(
                            "Transformation settings have configuration data for transformation '{0}', and that transformation cannot be found.",
                            child.Name));

                set = factory.GetDefaultSettings();
                _transformationSettings.Add(set);
            }
            set.ReadValuesFrom(child);
        }
        resetLookups();
    }

    public XElement ValuesToXElement()
    {
        return new XElement("RuntimeSettings",
                            from set in AllSettings
                            select set.ValuesToXElement());
    }

    public void SaveValues(string fileName)
    {
        new XDocument(ValuesToXElement()).Save(fileName);
    }

    public void LoadValues(string fileName, PublishingRuntime runtime)
    {
        ReadValuesFrom(XDocument.Load(fileName).Root, runtime);
    }

    public void Reset()
    {
        GlobalSettings.Reset();
        _transformationSettings.RemoveAll(set => set != GlobalSettings);
        resetLookups();
    }
}
