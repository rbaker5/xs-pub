using Microsoft.Extensions.DependencyInjection;
using Xsp;
using XsPub.Runtime;
using XsPub.Runtime.Transformations;

var services = new ServiceCollection();
services.AddSingleton<ITransformationFactory, AllToSequenceFactory>();
services.AddSingleton<ITransformationFactory, InlineGroupsFactory>();
services.AddSingleton<ITransformationFactory, ExplicitFormsFactory>();
services.AddSingleton<PublishingRuntime>();

var provider = services.BuildServiceProvider();
var runtime = provider.GetRequiredService<PublishingRuntime>();

try
{
    var parser = new ArgumentParser(args);
    parser.Parse();

    var defaultSettings = runtime.CreateDefaultSettings();
    foreach (var arg in parser.ArgumentSettings)
    {
        if (arg.TransformationName != null)
        {
            foreach (var set in defaultSettings.AllSettings
                         .Where(s => s.TransformationName == arg.TransformationName))
            {
                set.SetPropertyValueWithString(arg.SettingName, arg.Value);
            }
        }
        else
        {
            defaultSettings.GlobalSettings.SetPropertyValueWithString(arg.SettingName, arg.Value);
        }
    }

    runtime.Publish(parser.InputFile, parser.OutputPath, defaultSettings);
}
catch (ArgumentParseException e)
{
    Console.Error.WriteLine(e.Message);
    return 1;
}
catch (Exception e)
{
    Console.Error.WriteLine(e.ToString());
    return 1;
}

return 0;
