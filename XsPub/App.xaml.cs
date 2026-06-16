using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows;
using XsPub.Runtime;

namespace XsPub
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private CompositionContainer _container;

        [Import("MainWindow")]
        public new Window MainWindow
        {
            get { return base.MainWindow; }
            set { base.MainWindow = value; }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (Compose())
            {
                MainWindow.Show();
            }
            else
            {
                Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            if (_container != null)
            {
                _container.Dispose();
            }
        }

        private bool Compose()
        {
            //var directoryCatalog = new DirectoryCatalog(".");
            var appCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var runtimeCatalog = new AssemblyCatalog(typeof(PublishingRuntime).Assembly);
            var aggregateCatalog = new AggregateCatalog();
            //aggregateCatalog.Catalogs.Add(directoryCatalog);
            aggregateCatalog.Catalogs.Add(appCatalog);
            aggregateCatalog.Catalogs.Add(runtimeCatalog);

            _container = new CompositionContainer(aggregateCatalog);

            var batch = new CompositionBatch();
            batch.AddPart(this);

            try
            {
                _container.Compose(batch);
            }
            catch (CompositionException compositionException)
            {
                MessageBox.Show(compositionException.ToString());
                Shutdown(1);
                return false;
            }
            return true;
        }

    }
}
