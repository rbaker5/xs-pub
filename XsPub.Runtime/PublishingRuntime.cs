using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using XsPub.Library.Xml;
using XsPub.Library.Xml.Schema;
using XsPub.Runtime.Settings;

namespace XsPub.Runtime
{
    [Export(typeof(PublishingRuntime))]
    public class PublishingRuntime
    {
        [ImportMany]
        public IEnumerable<ITransformationFactory> TransformationFactories { get; set; }
        
        public FormattedXmlWriterSettings OutputSettings { get; private set; } 

        public PublishingRuntime()
        {
            OutputSettings = new FormattedXmlWriterSettings();
        }

        public RuntimeSettingSet CreateDefaultSettings()
        {
            return
                new RuntimeSettingSet(
                    new SettingSet("Global", "Global Settings", "Settings that apply to the entire runtime"),
                    Enumerable.Empty<SettingSet>());
        }

        /// <summary>
        /// Reads in an input schema and outputs it, along with any resolved dependencies
        /// to an output folder after applying transformations.
        /// </summary>
        /// <param name="schemaFilePath">
        /// The full path of an input XSD or WSDL file.
        /// </param>
        /// <param name="outputPath">
        /// The path of a folder where output files are written.
        /// </param>
        public void Publish(string schemaFilePath, string outputPath, RuntimeSettingSet settingSets = null)
        {
            Contract.Requires<ArgumentNullException>(schemaFilePath != null);
            Contract.Requires<ArgumentNullException>(outputPath != null);
            Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(schemaFilePath));

            if (settingSets == null)
                settingSets = CreateDefaultSettings();

            var document = XDocument.Load(schemaFilePath, LoadOptions.PreserveWhitespace | LoadOptions.SetBaseUri | LoadOptions.SetLineInfo);
            Contract.Assert(document.Root != null);

            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            if (document.Root.Name == Xs.Schema)
                publishSchema(document.Root, outputPath, false, settingSets);
            else if (document.Root.Name == Wsdl.Definitions)
                publishWsdl(schemaFilePath, outputPath, document, settingSets);
            else
                throw new InvalidOperationException(
                    "Input file was neither WSDL or XSD.  xs:schema or wsdl:definitions element not found.");
        }

        private void publishWsdl(string schemaFilePath, string outputPath, XDocument wsdlDocument, RuntimeSettingSet settings)
        {
            Contract.Requires<ArgumentNullException>(schemaFilePath != null);
            Contract.Requires<ArgumentNullException>(outputPath != null);
            Contract.Requires<ArgumentNullException>(wsdlDocument != null);
            Contract.Requires<ArgumentNullException>(settings != null);

            var schemas = wsdlDocument.Descendants(Xs.Schema);
            copyPrefixes(schemas, wsdlDocument);

            foreach (var schema in schemas.ToList())
                publishSchema(schema, outputPath, true, settings);

            // Copy WSDL to output directory
            Directory.CreateDirectory(outputPath);
            SaveDocument(wsdlDocument, Path.Combine(outputPath, Path.GetFileName(schemaFilePath)));
        }

        private void publishSchema(XElement schemaRoot, string outputPath, bool isEmbeddedInWsdl, RuntimeSettingSet settings)
        {
            Contract.Requires<ArgumentNullException>(schemaRoot != null);
            Contract.Requires<ArgumentNullException>(settings != null);

            var schema = XsSchema.Load(schemaRoot);
            var transformations = TransformationFactories.SelectMany(factory => factory.CreateTransformations(settings)).ToList();
            var operation = new PublishOperation(this, schema, outputPath, isEmbeddedInWsdl, transformations);
            operation.Publish();

            if (isEmbeddedInWsdl)
            {
                schemaRoot.ReplaceWith(schema.Element);
            }
        }

        private void copyPrefixes(IEnumerable<XElement> schemaElements, XDocument wsdlDocument)
        {
            Contract.Requires<ArgumentNullException>(schemaElements != null);
            Contract.Requires<ArgumentNullException>(wsdlDocument != null);

            var wsdlDefaultNamespace = wsdlDocument.Root.GetDefaultNamespace();
            var namespaces = from attribute in wsdlDocument.Root.Attributes()
                             where
                                 attribute.Name.NamespaceName == XNamespace.Xmlns
                             select new { Prefix = attribute.Name.LocalName, Name = attribute.Value };

            foreach (var schemaElement in schemaElements)
            {
                if (schemaElement.GetDefaultNamespace() == XNamespace.None && wsdlDefaultNamespace != XNamespace.None)
                    schemaElement.SetAttributeValue("xmlns", wsdlDefaultNamespace);

                // TODO: Shouldn't copy all indiscriminately.  Idealy should inspect the entire schema element to see if they are used.  
                // A little difficult because some (not all) attributes need to be inspected.
                foreach (var ns in namespaces)
                {
                    if (schemaElement.GetNamespaceOfPrefix(ns.Prefix) == null)
                    {
                        var usesNamespace = schemaElement.Descendants().Any(element => element.Name.NamespaceName == ns.Name);
                        if (schemaElement.Descendants().Attributes().Any(attribute => attribute.Value.StartsWith(ns.Prefix)))
                            usesNamespace = true;
                        //if (usesNamespace)
                        schemaElement.SetAttributeValue(XNamespace.Xmlns.GetName(ns.Prefix), ns.Name);
                    }
                }
            }
        }

        public void SaveDocument(XDocument document, string outputFileName)
        {
            Contract.Requires<ArgumentNullException>(document != null);
            Contract.Requires<ArgumentNullException>(outputFileName != null);

            if (OutputSettings.WrappedElements.Count == 0)
            {
                using (var writer = XmlWriter.Create(outputFileName, OutputSettings.WriterSettings))
                    document.Save(writer);
            }
            else
            {
                using (var stream = new MemoryStream())
                {
                    using (var writer = XmlWriter.Create(stream, OutputSettings.WriterSettings))
                        document.Save(writer);

                    var builder = new StringBuilder(OutputSettings.WriterSettings.Encoding.GetString(stream.ToArray()));

                    foreach (var wrappedElementNames in OutputSettings.WrappedElements)
                        addNewLinesToAttributes(builder, wrappedElementNames.LocalName);

                    File.WriteAllText(outputFileName, builder.ToString(), OutputSettings.WriterSettings.Encoding);
                }
            }
        }

        private void addNewLinesToAttributes(StringBuilder builder, string elementName)
        {
            Contract.Requires<ArgumentNullException>(builder != null);
            Contract.Requires<ArgumentNullException>(elementName != null);

            string originalText = builder.ToString();
            int schemaStart = -1;
            var validPrefixCharacters = new[] { ':', '<' };
            do
            {
                schemaStart = originalText.IndexOf(elementName + " ", schemaStart + 1);
                if (schemaStart == -1) return;
            }
            while (schemaStart == 0 || !validPrefixCharacters.Contains(originalText[schemaStart - 1]));

            var schemaLineStart = originalText.Substring(0, schemaStart).LastIndexOf('\n');
            // -1 would be fine, except for the Unicode preamble.
            if (schemaLineStart == -1) schemaLineStart = 0;
            var schemaElementStart = originalText.Substring(0, schemaStart).LastIndexOf('<');
            var newLineChars = OutputSettings.WriterSettings.NewLineChars + new string(' ', schemaElementStart - schemaLineStart + 2);
            var schemaEnd = originalText.IndexOf(">", schemaStart);

            var attributeEndIndexes = new List<int>();
            var nextSearchStart = schemaStart;

            while (nextSearchStart < schemaEnd && nextSearchStart != -1)
            {
                var nextAttributeStart = originalText.IndexOf('"', nextSearchStart);
                var nextAttributeEnd = originalText.IndexOf('"', nextAttributeStart + 1);
                attributeEndIndexes.Add(nextAttributeEnd);
                nextSearchStart = nextAttributeEnd + 1;
            }

            // Replace from end because if we do from start, it will alter later indexes.
            // Also, skip last one so that > is on line with last attribute.
            attributeEndIndexes.Reverse();
            foreach (var attributeEndIndex in attributeEndIndexes.Skip(1))
                builder.Insert(attributeEndIndex + 1, newLineChars);

            // Finally add line before first attribute.
            builder.Insert(schemaStart + elementName.Length, newLineChars);
        }
    }
}
