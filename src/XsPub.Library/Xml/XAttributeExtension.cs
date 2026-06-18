using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace XsPub.Library.Xml
{
    public static class XAttributeExtension
    {
        public static string ValueOrDefault(this XAttribute attribute)
        {
            if (attribute == null) return null;
            return attribute.Value;
        }

        public static string ValueOrDefault(this XAttribute attribute, string defaultValue)
        {
            if (attribute == null) return defaultValue;
            return attribute.Value;
        }
    }

    public static class XElementExtension
    {
        public static string ValueOrDefault(this XElement element)
        {
            if (element == null) return null;
            return element.Value;
        }

        public static string ValueOrDefault(this XElement element, string defaultValue)
        {
            if (element == null) return defaultValue;
            return element.Value;
        }
    }
}
