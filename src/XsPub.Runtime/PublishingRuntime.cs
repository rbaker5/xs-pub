using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using XsPub.Library.Xml;
using XsPub.Library.Xml.Schema;
using XsPub.Runtime.Settings;

namespace XsPub.Runtime;

public class PublishingRuntime
{
    public IEnumerable<ITransformationFactory> TransformationFactories { get; }

    public FormattedXmlWriterSettings OutputSettings { get; }

    public PublishingRuntime(IEnumerable<ITransformationFactory> transformationFactories)
    {
        TransformationFactories = transformationFactories;
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
    /// <param name="allowExternalResources">
    /// When false (the default), xs:import and xs:include are restricted to local files
    /// within the same directory tree as the input file.  Set to true only for fully
    /// trusted input that legitimately references remote schemas or absolute file paths.
    /// </param>
    public void Publish(string schemaFilePath, string outputPath,
                        RuntimeSettingSet? settingSets = null,
                        bool allowExternalResources = false)
    {
        ArgumentNullException.ThrowIfNull(schemaFilePath);
        ArgumentNullException.ThrowIfNull(outputPath);
        ArgumentException.ThrowIfNullOrEmpty(schemaFilePath);

        settingSets ??= CreateDefaultSettings();

        var fullInputPath = Path.GetFullPath(schemaFilePath);
        XmlResolver resolver = allowExternalResources
            ? new XmlUrlResolver()
            : new LocalOnlyXmlResolver(Path.GetDirectoryName(fullInputPath)!);

        var document = XDocument.Load(fullInputPath, LoadOptions.PreserveWhitespace | LoadOptions.SetBaseUri | LoadOptions.SetLineInfo);

        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        if (document.Root!.Name == Xs.Schema)
            publishSchema(document.Root, outputPath, false, settingSets, resolver);
        else if (document.Root.Name == Wsdl.Definitions)
            publishWsdl(fullInputPath, outputPath, document, settingSets, resolver);
        else
            throw new InvalidOperationException(
                "Input file was neither WSDL or XSD.  xs:schema or wsdl:definitions element not found.");
    }

    private void publishWsdl(string schemaFilePath, string outputPath, XDocument wsdlDocument,
                              RuntimeSettingSet settings, XmlResolver resolver)
    {
        ArgumentNullException.ThrowIfNull(schemaFilePath);
        ArgumentNullException.ThrowIfNull(outputPath);
        ArgumentNullException.ThrowIfNull(wsdlDocument);
        ArgumentNullException.ThrowIfNull(settings);

        var schemas = wsdlDocument.Descendants(Xs.Schema);
        copyPrefixes(schemas, wsdlDocument);

        foreach (var schema in schemas.ToList())
            publishSchema(schema, outputPath, true, settings, resolver);

        Directory.CreateDirectory(outputPath);
        SaveDocument(wsdlDocument, Path.Join(outputPath, Path.GetFileName(schemaFilePath)));
    }

    private void publishSchema(XElement schemaRoot, string outputPath, bool isEmbeddedInWsdl,
                                RuntimeSettingSet settings, XmlResolver resolver)
    {
        ArgumentNullException.ThrowIfNull(schemaRoot);
        ArgumentNullException.ThrowIfNull(settings);

        var schema = XsSchema.Load(schemaRoot);
        schema.XmlResolver = resolver;
        var transformations = TransformationFactories.SelectMany(factory => factory.CreateTransformations(settings)).ToList();
        var operation = new PublishOperation(this, schema, outputPath, isEmbeddedInWsdl, transformations);
        operation.Publish();

        if (isEmbeddedInWsdl)
            schemaRoot.ReplaceWith(schema.Element);
    }

    private void copyPrefixes(IEnumerable<XElement> schemaElements, XDocument wsdlDocument)
    {
        ArgumentNullException.ThrowIfNull(schemaElements);
        ArgumentNullException.ThrowIfNull(wsdlDocument);

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
            foreach (var ns in namespaces.Where(ns => schemaElement.GetNamespaceOfPrefix(ns.Prefix) == null))
            {
                var usesNamespace = schemaElement.Descendants().Any(element => element.Name.NamespaceName == ns.Name);
                if (schemaElement.Descendants().Attributes().Any(attribute => attribute.Value.StartsWith(ns.Prefix)))
                    usesNamespace = true;
                //if (usesNamespace)
                schemaElement.SetAttributeValue(XNamespace.Xmlns.GetName(ns.Prefix), ns.Name);
            }
        }
    }

    public void SaveDocument(XDocument document, string outputFileName)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(outputFileName);

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
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(elementName);

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
