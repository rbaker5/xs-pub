using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using XsPub.Runtime;
using XsPub.Runtime.Settings;
using XsPub.Utility;

namespace XsPub
{
    /// <summary>
    /// Interaction logic for PublishWindow.xaml
    /// </summary>
    [Export("MainWindow", typeof(Window))]
    public partial class PublishWindow : Window
    {
        [Import(RequiredCreationPolicy = CreationPolicy.NonShared)]
        public PublishingRuntime Runtime { get; set; }

        public RuntimeSettingSet Settings
        {
            get { return (RuntimeSettingSet)GetValue(SettingsProperty); }
            set { SetValue(SettingsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Settings.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SettingsProperty =
            DependencyProperty.Register("Settings", typeof(RuntimeSettingSet), typeof(PublishWindow), new UIPropertyMetadata(null));

        public IEnumerable<ITransformationFactory> Factories
        {
            get { return (IEnumerable<ITransformationFactory>)GetValue(FactoriesProperty); }
            set { SetValue(FactoriesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Settings.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FactoriesProperty =
            DependencyProperty.Register("Factories", typeof(IEnumerable<ITransformationFactory>), typeof(PublishWindow), new UIPropertyMetadata(null));

        public PublishWindow()
        {
            InitializeComponent();
        }

        private void publish_Click(object sender, RoutedEventArgs e)
        {
            Runtime.Publish(inputFilePath.Text, outputPath.Text, Settings);
        }

        private void _this_Loaded(object sender, RoutedEventArgs e)
        {
            Settings = Runtime.CreateDefaultSettings();
            Factories = Runtime.TransformationFactories;
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var row = DataGridUtility.TryFindParent<DataGridRow>(e.OriginalSource as DependencyObject);
            if (row == null)
                return;

            var factory = row.Item as ITransformationFactory;
            if (factory == null)
                return;

            if (factory.IsSingleton)
            {
                if (Settings.SetsByTransformation.Contains(factory.Name))
                    MessageBox.Show(string.Format("{0} is already configured and can be used once.", factory.DisplayName));

                Settings.Add(factory.GetDefaultSettings());
            }
            else
            {
                Settings.Add(factory.GetDefaultSettings());
            }

            _settings.Items.Refresh();
            
            e.Handled = true;
        }

        private void _loadButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            var result = dialog.ShowDialog();
            if (result.GetValueOrDefault())
            {
                Settings.LoadValues(dialog.FileName, Runtime);
            }
            _settings.Items.Refresh();
        }

        private void _saveButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();
            var result = dialog.ShowDialog();
            if (result.GetValueOrDefault())
            {
                Settings.SaveValues(dialog.FileName);
            }
        }

        private void _resetButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.Reset();
            _settings.Items.Refresh();
        }
    }
}
