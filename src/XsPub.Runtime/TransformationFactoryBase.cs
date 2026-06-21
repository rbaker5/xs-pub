using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XsPub.Runtime.Settings;

namespace XsPub.Runtime;

public abstract class TransformationFactoryBase : ITransformationFactory
{
    public abstract bool IsSingleton { get; }
    public abstract string Name { get; }
    public abstract string DisplayName { get; }
    public abstract string Description { get; }
    public virtual SettingSet GetDefaultSettings()
    {
        return new SettingSet(Name, DisplayName, Description);
    }
    public abstract IEnumerable<ITransformation> CreateTransformations(RuntimeSettingSet settings);
}
