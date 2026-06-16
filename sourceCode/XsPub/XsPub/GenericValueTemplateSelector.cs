using System;
using System.Collections;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace XsPub
{
    internal class GenericValueTemplateSelector : DataTemplateSelector
    {
        public TemplateList Templates { get; set; }
        public int GenericParameterIndex { get; set; }

        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            var element = container as FrameworkElement;
            if (element == null)
                return null;

            if (item == null)
                return null;

            var itemType = item.GetType();
            if (!itemType.IsGenericType) 
                return null;

            var genericArguments = itemType.GetGenericArguments();
            if (genericArguments.Length <= GenericParameterIndex)
                throw new InvalidOperationException(
                    string.Format(
                        "GenericValueTemplateSelector Failure: Unable to find template because there is not a generic type parameter with index: {0}.",
                        GenericParameterIndex));

            DataTemplate dataTemplate;
            var templateType = genericArguments[GenericParameterIndex];
           
            if (Templates.Index.TryGetValue(templateType, out dataTemplate))
                return dataTemplate;

            if (typeof(IList).IsAssignableFrom(templateType))
            {
                if (Templates.Index.TryGetValue(typeof(IList), out dataTemplate))
                    return dataTemplate;
            }

            if (Templates.Index.TryGetValue(typeof(string), out dataTemplate))
                return dataTemplate;

            return null;
        }
    }
}
