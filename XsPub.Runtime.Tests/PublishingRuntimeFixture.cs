using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using XsPub.Runtime.Transformations;

namespace XsPub.Runtime.Tests
{
    [TestFixture]
    public class PublishingRuntimeFixture
    {
        private CompositionContainer _container;

        [SetUp]
        public void SetupContainer()
        {
            var runtimeCatalog = new AssemblyCatalog(typeof(PublishingRuntime).Assembly);
            var aggregateCatalog = new AggregateCatalog();
            aggregateCatalog.Catalogs.Add(runtimeCatalog);
            _container = new CompositionContainer(aggregateCatalog);
        }

        [Test]
        public void NoInput()
        {   
            var runtime = _container.GetExportedValue<PublishingRuntime>();
            Assert.Throws(typeof(ArgumentNullException), () => runtime.Publish(null, null));
        }

        [Test]
        public void PublishTestWsdls()
        {
            var runtime = _container.GetExportedValue<PublishingRuntime>();
            var testFiles = TestFiles.GetXsdAndWsdlFiles();
            foreach (var file in testFiles.Input)
                runtime.Publish(file.FullName, testFiles.OutputDirectory.FullName);

            testFiles.CompareOutput();
        }

        [Test]
        public void PublishInlinedGroupsTestWsdls()
        {
            var runtime = _container.GetExportedValue<PublishingRuntime>();
            var testFiles = TestFiles.GetXsdAndWsdlFiles("InlineGroups");
            var settings = runtime.CreateDefaultSettings();
            settings.AddRange(runtime.TransformationFactories.Where(factory => factory.Name == "InlineGroups").Select(factory => factory.GetDefaultSettings()));
            foreach (var file in testFiles.Input)
                runtime.Publish(file.FullName, testFiles.OutputDirectory.FullName, settings);

            testFiles.CompareOutput();
        }
    }
}
