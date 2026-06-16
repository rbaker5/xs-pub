using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Text;
using XsPub.Runtime;

namespace Xsp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var parser = new ArgumentParser(args);
                parser.Parse();

                var container = Compose();
                var runtime = container.GetExport<PublishingRuntime>().Value;
                var defaultSettings = runtime.CreateDefaultSettings();
                foreach (var argumentSetting in parser.ArgumentSettings)
                {
                    if (argumentSetting.TransformationName != null)
                    {
                        foreach (
                            var set in
                                Enumerable.Repeat(defaultSettings.SetsByName[argumentSetting.TransformationName], 1).Concat(
                                    defaultSettings.SetsByTransformation[argumentSetting.TransformationName]))
                        {
                            set.SetPropertyValueWithString(argumentSetting.SettingName, argumentSetting.Value);
                        }
                    }
                    else
                    {
                        defaultSettings.GlobalSettings.SetPropertyValueWithString(argumentSetting.SettingName, argumentSetting.Value);
                    }
                }
                runtime.Publish(parser.InputFile, parser.OutputPath, defaultSettings);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static CompositionContainer Compose()
        {
            //var directoryCatalog = new DirectoryCatalog(".");
            var appCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var runtimeCatalog = new AssemblyCatalog(typeof(PublishingRuntime).Assembly);
            var aggregateCatalog = new AggregateCatalog();
            //aggregateCatalog.Catalogs.Add(directoryCatalog);
            aggregateCatalog.Catalogs.Add(appCatalog);
            aggregateCatalog.Catalogs.Add(runtimeCatalog);

            var container = new CompositionContainer(aggregateCatalog);
            return container;
        }
    }
}
