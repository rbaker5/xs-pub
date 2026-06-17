using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using XsPub.Library.Xml.Schema.Collections;

namespace XsPub.Library.Xml.Schema
{
    public abstract class XsObject
    {
        private readonly static XNamespace[] _processedNamespaces = new[] { Xml.Namespaces.Xs, XNamespace.Xmlns, XNamespace.None };
        protected IEnumerable<XNamespace> ProcessedNamespaces { get { return _processedNamespaces; } }

        protected XsObject(XElement element, XsObject parent = null)
        {
            Element = element;
            if (parent != null) SetParent(parent);
        }

        private XsObject _parent;
        public XsObject Parent
        {
            get
            {
                if (_parent == null && Element.Parent != null)
                {
                    _parent = XsObjectFactory.Create<XsObject>(Element.Parent);
                }
                return _parent;
            }
            private set { _parent = value; }
        }

        public XElement Element { get; protected set; }
        public int LineNumber { get { return ((IXmlLineInfo) Element).LineNumber; }}
        public int LinePosition { get { return ((IXmlLineInfo)Element).LinePosition; }}
        public string SourceUri { get { return Element.BaseUri; } }
        public NamespaceCollection Namespaces { get { return new NamespaceCollection(this); }}

        protected void SetParent(XsObject parent)
        {
            Parent = parent;
        }

        internal XmlSerializerNamespaces CreateNamespaces()
        {
            var namespaces = new XmlSerializerNamespaces(Parent.CreateNamespaces());
            foreach (var localNamespace in Element.Attributes().Where(attribute => attribute.IsNamespaceDeclaration))
            {
                if (localNamespace.Name.Namespace == XNamespace.Xmlns)
                    namespaces.Add(localNamespace.Name.LocalName, localNamespace.Value);
                else
                    namespaces.Add(string.Empty, localNamespace.Value);
            }
            return namespaces;
        }

        internal IEnumerable<XAttribute> GetUnhandledAttributes()
        {
            return Element.Attributes().Where(attribute => !ProcessedNamespaces.Contains(attribute.Name.Namespace));
        }

        #region Element Helpers
        internal T GetElement<T>(XName name) where T : XsObject
        {
            return XsObjectFactory.Create<T>(Element.Element(name), this);
        }

        internal void SetElement<T>(XName name, T value)
            where T : XsObject
        {
            var element = Element.Element(name);
            if (element == null)
            {
                Element.Add(value.Element);
            }
            else
            {
                element.ReplaceWith(value.Element);
            }
        }

        internal T GetExclusiveOrElement<T>(IEnumerable<XName> names) where T : XsObject
        {
            return XsObjectFactory.Create<T>(Element.Elements().Where(element => names.Contains(element.Name)).SingleOrDefault(), this);
        }

        internal void SetExclusiveOrElement<T>(IEnumerable<XName> names, T value) where T : XsObject
        {
            var foundElement = Element.Elements().Where(element => names.Contains(element.Name)).SingleOrDefault();
            if (foundElement == null)
            {
                Element.Add(value.Element);
            }
            else
            {
                foundElement.ReplaceWith(value.Element);
            }
        }
        #endregion Element Helpers

        #region Attribute Helpers
        #region Generic Attribute Helper
        internal void SetAttributeValueInternal<T>(XName name, T value)
            where T : struct
        {
            Element.SetAttributeValue(name, value);
        }

        internal void SetAttributeValueInternal<T>(XName name, T value, T defaultValue)
            where T : struct
        {
            Element.SetAttributeValue(name, value.Equals(defaultValue) ? null : (object)value );
        }
        #endregion Generic Attribute Helper

        #region String Attribute Helpers
        internal string GetAttributeValueInternal(XName name)
        {
            // TODO: Figure out (and test) all the empty/null scenarios.
            return Element.Attribute(name).ValueOrDefault(); //.ValueOrDefault(string.Empty);
        }

        internal void SetAttributeValueInternal(XName name, string value)
        {
            Element.SetAttributeValue(name, value);
        }

        internal string GetAttributeValueInternal(XName name, string defaultValue)
        {
            // TODO: Figure out (and test) all the empty/null scenarios.
            return Element.Attribute(name).ValueOrDefault(defaultValue);
        }

        internal void SetAttributeValueInternal(XName name, string value, string defaultValue)
        {
            Element.SetAttributeValue(name, value == defaultValue ? null : value);
        }
        #endregion String Attribute Helpers

        #region Generic Attribute Helper Specializations
        #region Bool
        // Use with SetAttributeValueInternal<T>(XName, T)
        internal bool GetAttributeValueInternal(XName name, bool defaultValue)
        {
            var attribute = Element.Attribute(name);
            if (attribute == null) return defaultValue;

            bool returnValue;
            if (!bool.TryParse(attribute.Value, out returnValue))
                throw createInvalidFormatException(name);

            return returnValue;
        }
        #endregion

        #region XmlSchemaContentProcessing
        internal XmlSchemaContentProcessing GetAttributeValueInternal(XName name, XmlSchemaContentProcessing defaultValue)
        {
            var attribute = Element.Attribute(name);
            if (attribute == null) return defaultValue;

            var enumValue = EnumConverter.ParseContentProcessing(attribute.Value);
            if (enumValue == XmlSchemaContentProcessing.None)
                throw new XsException(string.Format("Unable to read '{0}' because the format is invalid.", name), this);

            return enumValue;
        }

        internal void SetAttributeValueInternal(XName name, XmlSchemaContentProcessing value, XmlSchemaContentProcessing defaultValue)
        {
            Element.SetAttributeValue(name, value == defaultValue ? null : EnumConverter.ToString(value));
        }
        #endregion XmlSchemaContentProcessing

        #region XmlSchemaDerivationMethod
        internal XmlSchemaDerivationMethod GetAttributeValueInternal(XName name, XmlSchemaDerivationMethod defaultValue)
        {
            var attribute = Element.Attribute(name);
            if (attribute == null) return defaultValue;

            var enumValue = EnumConverter.ParseDerivationMethod(attribute.Value);
            if (enumValue == XmlSchemaDerivationMethod.None)
                throw new XsException(string.Format("Unable to read '{0}' because the format is invalid.", name), this);

            return enumValue;
        }

        internal void SetAttributeValueInternal(XName name, XmlSchemaDerivationMethod value, XmlSchemaDerivationMethod defaultValue)
        {
            Element.SetAttributeValue(name, value == defaultValue ? null : EnumConverter.ToString(value));
        }
        #endregion XmlSchemaDerivationMethod

        #region XmlSchemaForm
        internal XmlSchemaForm GetAttributeValueInternal(XName name, XmlSchemaForm defaultValue)
        {
            var attribute = Element.Attribute(name);
            if (attribute == null) return defaultValue;

            var enumValue = EnumConverter.ParseForm(attribute.Value);
            if (enumValue == XmlSchemaForm.None)
                throw new XsException(string.Format("Unable to read '{0}' because the format is invalid.", name), this);

            return enumValue;
        }

        internal void SetAttributeValueInternal(XName name, XmlSchemaForm value, XmlSchemaForm defaultValue)
        {
            Element.SetAttributeValue(name, value == defaultValue ? null : EnumConverter.ToString(value));
        }
        #endregion XmlSchemaForm

        #region XmlSchemaUse
        internal XmlSchemaUse GetAttributeValueInternal(XName name, XmlSchemaUse defaultValue)
        {
            var attribute = Element.Attribute(name);
            if (attribute == null) return defaultValue;

            var enumValue = EnumConverter.ParseUse(attribute.Value);
            if (enumValue == XmlSchemaUse.None)
                throw new XsException(string.Format("Unable to read '{0}' because the format is invalid.", name), this);

            return enumValue;
        }

        internal void SetAttributeValueInternal(XName name, XmlSchemaUse value, XmlSchemaUse defaultValue)
        {
            Element.SetAttributeValue(name, value == defaultValue ? null : EnumConverter.ToString(value));
        }
        #endregion XmlSchemaUse
        #endregion Generic Attribute Helper Specializations
        #endregion Attribute Helpers

        #region Name Get/Set helpers
        internal XName GetLocalGlobalOrReferencedName(XName name, XName refName, XmlSchemaForm form, Func<XsSchema, XmlSchemaForm> formDefaultAccessor)
        {
            var localOrGlobalName = (Parent is XsSchema) ? GetGlobalQualifiedName(name) : GetLocalQualifiedName(name, form, formDefaultAccessor);
            return localOrGlobalName ?? GetQualifiedName(refName);
        }
        
        /// <summary>
        /// Used to convert a name of an declared object.  Objects are declared in the target namespace.
        /// </summary>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        internal XName GetGlobalQualifiedName(XName attributeName)
        {
            var localName = GetAttributeValueInternal(attributeName);
            if (localName == null) return null;

            var schema = GetSchema();
            return schema == null ? XNamespace.None.GetName(localName) : XNamespace.Get(schema.TargetNamespace).GetName(localName);
        }

        internal XName GetLocalQualifiedName(XName attributeName, XmlSchemaForm form, Func<XsSchema, XmlSchemaForm> formDefaultAccessor)
        {
            var localName = GetAttributeValueInternal(attributeName);
            if (localName == null) return null;

            var schema = GetSchema();
            if (schema == null || form == XmlSchemaForm.Unqualified || (form == XmlSchemaForm.None) && (formDefaultAccessor(schema) != XmlSchemaForm.Qualified))
                return XNamespace.None.GetName(localName);
            
            return XNamespace.Get(schema.TargetNamespace).GetName(localName);
        }

        internal XName GetQualifiedName(XName attributeName)
        {
            var attribute = Element.Attribute(attributeName);
            if (attribute == null) return null;

            var name = attribute.Value;
            if (string.IsNullOrEmpty(name)) return null;
            var nameParts = name.Split(new[] { ':' }, 2);
            string localName;
            XNamespace resolvedNamespace;
            if (nameParts.Length == 1)
            {
                localName = nameParts[0];
                resolvedNamespace = Element.GetDefaultNamespace();
            }
            else
            {
                localName = nameParts[1];
                resolvedNamespace = Element.GetNamespaceOfPrefix(nameParts[0]);
            }
            return resolvedNamespace == null ? XNamespace.None.GetName(name) : resolvedNamespace.GetName(localName);
        }

        internal void SetQualifiedName(XName attributeName, XName qualifiedName)
        {
            Element.SetAttributeValue(attributeName, convertQualifiedName(qualifiedName));
        }

        private string convertQualifiedName(XName qualifiedName)
        {
            if (qualifiedName.Namespace == XNamespace.None || qualifiedName.Namespace == Element.GetDefaultNamespace()) 
                return qualifiedName.LocalName;

            var prefix = Element.GetPrefixOfNamespace(qualifiedName.Namespace);
            if (string.IsNullOrEmpty(prefix)) 
                throw new InvalidOperationException("Namespace did not resolve to a valid prefix.");

            return prefix + ":" + qualifiedName.LocalName;
        }
        #endregion

        internal XsSchema GetSchema()
        {
            XsObject schemaSearch = this;
            while (schemaSearch != null && !(schemaSearch is XsSchema)) schemaSearch = schemaSearch.Parent;
            return schemaSearch as XsSchema;
        }

        internal XmlSchemaDerivationMethod ResolveDerivationMethod(XName name, XmlSchemaDerivationMethod filter, Func<XsSchema, XmlSchemaDerivationMethod> defaultSelector)
        {
            var localValue = GetAttributeValueInternal(name, XmlSchemaDerivationMethod.None);
            if (localValue == XmlSchemaDerivationMethod.None)
            {
                localValue = GetSchema().IfNotNull(defaultSelector, XmlSchemaDerivationMethod.None);
                if (localValue != XmlSchemaDerivationMethod.All) 
                    localValue &= filter;
            }
            else
            {
                if ((localValue & ~filter) != XmlSchemaDerivationMethod.Empty)
                    throw createInvalidFormatException(name);

                localValue &= filter;
            }
            return localValue;
        }

        private XsException createInvalidFormatException(XName name)
        {
            return new XsException(
                string.Format(CultureInfo.InvariantCulture, "Unable to read '{0}' because the format is invalid.", name), this);
        }

        public DescendentCollection<XsObject> Descendents
        {
            get { return new DescendentCollection<XsObject>(this); }
        }


        // Could benefit from some validity checks
        public void ReplaceWith(XsObject obj)
        {
            Element.ReplaceWith(obj.Element);
        }

        public void ReplaceWith(params XsObject[] items)
        {
            ReplaceWith(items);
        }

        public void ReplaceWith(IEnumerable<XsObject> items)
        {
            Element.ReplaceWith(items.Select(obj => obj.Element));
        }

        public void Remove()
        {
            Element.Remove();
        }

        public void SetAttributeValue(XName attributeName, string value)
        {
            if (!IsValidAttribute(attributeName))
                throw new XsException(string.Format(
                    "The attribute '{0}' is not supported by the schema element '{1}'.", attributeName, Element.Name));

            SetAttributeValueInternal(attributeName, value);
        }

        protected virtual bool IsValidAttribute(XName attributeName)
        {
            return !ProcessedNamespaces.Contains(attributeName.Namespace);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("<");
            builder.Append(Element.Name.LocalName);
            foreach (var attribute in Element.Attributes())
            {
                builder.Append(" ");
                builder.Append(attribute.Name);
                builder.Append("=\"");
                builder.Append(attribute.Value);
                builder.Append("\"");
            }
            builder.Append("/>");
            return builder.ToString();
        }
    }
}