using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using XsPub.Library.Xml.Schema;

namespace XsPub.Runtime
{
    public class PublishOperation
    {
        public PublishingRuntime Runtime { get; private set; }
        public XsSchema Schema { get; private set; }
        public string OutputPath { get; private set; }
        public IEnumerable<ITransformation> Transformations { get; private set; }
        public bool IsEmbeddedInWsdl { get; set; }
        
        public PublishOperation(PublishingRuntime runtime, XsSchema schema, string outputPath, bool isEmbeddedInWsdl, IEnumerable<ITransformation> transformations)
        {
            Runtime = runtime;
            Schema = schema;
            OutputPath = outputPath;
            Transformations = transformations;
            IsEmbeddedInWsdl = isEmbeddedInWsdl;
        }

        private void publishChild(XsSchema schema)
        {
            var childOperation = new PublishOperation(Runtime, schema, OutputPath, IsEmbeddedInWsdl, Transformations);
            childOperation.Publish();
        }

        public void Publish()
        {
            var includes = Schema.Includes.ToList();
            foreach (var schemaReference in includes)
                publishChild(schemaReference.Schema);

            foreach (var transformation in Transformations)
                transformation.GatherData(Schema);

            foreach (var transformation in Transformations)
                transformation.IndependentTransform(Schema);

            foreach (var transformation in Transformations)
                transformation.DependentTransform(Schema);

            if (!IsEmbeddedInWsdl)
            {
                var sourceUri = new Uri(Schema.SourceUri);
                var fileName = sourceUri.Segments.Last();
                fileName = Path.ChangeExtension(fileName, ".xsd");

                Runtime.SaveDocument(Schema.Element.Document, Path.Combine(OutputPath, fileName));
            }
        }

        public void Validate()
        {
            
        }
    }
}