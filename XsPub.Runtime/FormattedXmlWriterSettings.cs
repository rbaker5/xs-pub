using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using XsPub.Library.Xml;

namespace XsPub.Runtime
{
    public class FormattedXmlWriterSettings
    {
        /// <summary>
        /// Settings to be used when writing output.
        /// </summary>
        public XmlWriterSettings WriterSettings { get; private set; }

        /// <summary>
        /// Names of elements which should put each attribute on a new line.
        /// </summary>
        public HashSet<XName> WrappedElements { get; private set; }

        public FormattedXmlWriterSettings()
        {
            WrappedElements = new HashSet<XName> {Wsdl.Definitions, Xs.Schema};
            WriterSettings = new XmlWriterSettings {Indent = false, NamespaceHandling = NamespaceHandling.Default, Encoding = new UTF8Encoding(false)};
        }
    }
}