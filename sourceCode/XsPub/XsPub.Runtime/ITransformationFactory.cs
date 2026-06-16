using System.Collections.Generic;
using System.Linq;
using System.Threading;
using XsPub.Runtime.Settings;

namespace XsPub.Runtime
{
    public interface ITransformationFactory
    {
        bool IsSingleton { get; }
        string Name { get; }
        string DisplayName { get; }
        string Description { get; }
        SettingSet GetDefaultSettings();
        IEnumerable<ITransformation> CreateTransformations(RuntimeSettingSet settings);
    }
}