# XsPub

XsPub helps manage XSD schemas and WSDL documents by providing an extensible framework to perform transformations to "publish" a schema.

By publishing a schema you can target different uses, such as documentation or input to code generation.

## Publishing

Why would you want to publish a schema or WSDL? A really descriptive and strict schema can be useful, but also an Achilles heel. Schemas and WSDLs are often fed into development tools to generate code and when schema elements like `choice` are used the output can be a bit weird. In addition, the outputted code will be very fragile from version to version. Often it is not a requirement, or even desirable for the applications which consume the schema to validate at this level of strictness. On the other hand, that level of strictness is a great tool when performing testing.

With XsPub you can get the best of both worlds and maintain a strict schema, and then through a series of transformations you select, generate a schema intended for more general consumption that relaxes rules to either make tools work, make generated code better, or make versioning more realistic.

## Usage

```
xsp <input.xsd|input.wsdl> <output-dir> [transformation.setting=value ...]
```

**Example — inline all group references before handing the schema to a code generator:**

```
xsp MySchema.xsd ./published InlineGroups.InlineGroups=true
```

### Built-in Transformations

| Name | Description |
|------|-------------|
| `AllToSequence` | Replaces `xs:all` with `xs:sequence` for broader tool compatibility |
| `InlineGroups` | Replaces `xs:group ref="…"` with the group's content |
| `ExplicitForms` | Ensures `attributeFormDefault` and `elementFormDefault` are explicitly set |

## Building

Requires [.NET 10 SDK](https://dot.net).

```
dotnet build
dotnet test
dotnet run --project Xsp -- <input> <output>
```

## Projects

| Project | Description |
|---------|-------------|
| `XsPub.Library` | XSD/WSDL schema object model |
| `XsPub.Runtime` | Transformation engine and pipeline |
| `Xsp` | Command-line interface |
| `XsPub.Tests` | xUnit test suite |

---
*Migrated to .NET 10 from the original .NET Framework 4.0 CodePlex source. Original archive preserved on the [archive/codeplex](../../tree/archive/codeplex) branch.*
