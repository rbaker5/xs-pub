using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using System.Xaml;
using System.Xml.Linq;
using XamlReader = System.Windows.Markup.XamlReader;

namespace XsPub
{
    internal class DefaultTemplateList : TemplateList
    {
        private static readonly XNamespace XamlPresentationNamespace = @"http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        private static readonly XNamespace XamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";

        private string _bindingPath;
        public string BindingPath
        {
            get { return _bindingPath; }
            set
            {
                _bindingPath = value;
                Clear();
                Init();
            }
        }

        protected void Init()
        {
            addBasicTemplate<string>("TextBox", "Text");
            addBasicTemplate<bool>("CheckBox", "IsChecked",
                                   new XAttribute("HorizontalAlignment", "Center"),
                                   new XAttribute("VerticalAlignment", "Center"));
            addBasicTemplate<List<string>>("ListBox", "ItemsSource");
            addBasicTemplate<IList>("DataGrid", "ItemsSource");
        }

        private void addBasicTemplate<T>(string uiElementType, string uiDataField, params XAttribute[] additionalAttributes)
        {
            addBasicTemplate(typeof(T), uiElementType, uiDataField, additionalAttributes);
        }

        private void addBasicTemplate(Type dataType, string uiElementType, string uiDataField, params XAttribute[] additionalAttributes)
        {
            var templateElement =
                createTemplate(dataType,
                               new XElement(XamlPresentationNamespace + uiElementType, createBinding(uiDataField), additionalAttributes));

            Add(convertToTemplate(templateElement));
        }

        private XElement createTemplate(Type dataType, XElement content)
        {
            var namespaces = new XamlNamespaceList();
            var dataTypeString = createTypeDeclaration(dataType, namespaces);

            return new XElement(XamlPresentationNamespace + "DataTemplate",
                                new XAttribute("xmlns", XamlPresentationNamespace),
                                new XAttribute(XNamespace.Xmlns.GetName("x"), XamlNamespace),
                                namespaces.GetAttributes(),
                                new XAttribute("DataType", string.Format("{{x:Type {0}}}", dataTypeString)),
                                content);
        }

        private string createTypeDeclaration(Type dataType, XamlNamespaceList namespaceList)
        {
            var prefix = namespaceList.GetPrefix(dataType);
            if (!dataType.IsGenericType)
                return string.Format("{0}:{1}", prefix, dataType.Name);

            var arguments = dataType.GetGenericArguments();
            var argumentString = string.Join(",", arguments.Select(argument => createTypeDeclaration(argument, namespaceList)));

            var genericStemEnd = dataType.Name.IndexOf('`');
            return string.Format("{0}:{1}({2})", prefix, dataType.Name.Substring(0, genericStemEnd), argumentString);
        }

        private class XamlNamespaceList
        {
            private Dictionary<string, string> _namespaces = new Dictionary<string, string>();

            public string GetPrefix(Type dataType)
            {
                string namespaceString = string.Format("clr-namespace:{0};assembly={1}",
                                                       dataType.Namespace,
                                                       dataType.Assembly.FullName);
                string prefix;
                if (_namespaces.TryGetValue(namespaceString, out prefix))
                    return prefix;

                prefix = "clr" + _namespaces.Count;
                _namespaces.Add(namespaceString, prefix);
                return prefix;
            }

            public IEnumerable<XAttribute> GetAttributes()
            {
                return _namespaces.Select(values => new XAttribute(XNamespace.Xmlns.GetName(values.Value), values.Key));
            }
        }


        private XAttribute createBinding(string propertyName)
        {
            return new XAttribute(propertyName, string.Format("{{Binding Path={0}}}", _bindingPath));
        }

        private DataTemplate convertToTemplate(XElement element)
        {
            using (var xmlReader = element.CreateReader())
            {
                return XamlReader.Load(xmlReader) as DataTemplate;
            }
        }
    }
}