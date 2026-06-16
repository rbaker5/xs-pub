**Project Description**
XsPub helps manage XSD schemas and WSDL documents by providing a extensible framework to perform transformations to "publish" a schema.

By publishing a schema you can target different uses.. such as documentation, or input to code generation.

**Publishing**
Why would you want to publish a schema or WSDL?  A really descriptive and strict schema can be useful, but also an Achilles heel.  Schemas and WSDLs are often fed into development tools to generate code and when schema elements like choice are used the output can be a bit weird.  In addition, the outputted code will be very fragile from version to version.  Often it's not a requirement, or even desirable for the applications which consume the schema to validate at this level of strictness.  On the other hand, that level of strictness is a great tool when performing testing.

With XsPub you can get the best of both worlds and maintain a strict schema, and then through a series of transformations you select, generate a schema intended for more general consumption that relaxes rules to either make tools work, make generated code better, or make versioning more realistic.