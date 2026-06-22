# xs-pub

XSD schema publishing tool. Loads XSD/WSDL schemas and writes transformed output
files via a pipeline of configurable transformations.

## Build and test

```
dotnet build XsPub.slnx
dotnet test XsPub.slnx
```

Build must be clean at **0 warnings, 0 errors** (`TreatWarningsAsErrors=true`).

## Layout

```
src/
  XsPub.Library/   Schema model (XsObject hierarchy, XML I/O helpers)
  XsPub.Runtime/   Transformation pipeline, runtime settings
  Xsp/             CLI entry point (xsp command)
tests/
  XsPub.Tests/         Integration and schema-model tests
  XsPub.Library.Tests/ Unit tests for library types
```

## Key classes

- `XsSchema` — entry point for loading a schema: `XsSchema.Load(path)`
- `XsObjectFactory` — maps `XElement` → `XsObject` subclass; keeps a weak-ref
  cache so the same element always returns the same object
- `PublishingRuntime` — owns the transformation pipeline; call `Publish()`
- `LocalOnlyXmlResolver` — blocks HTTP and path-traversal imports by default;
  pass `--allow-external` on the CLI to opt out

## Adding a transformation

1. Create a class in `src/XsPub.Runtime/Transformations/` that implements
   `ITransformation` (or inherits `TransformationBase`).
2. Register it in `PublishingRuntime.CreateDefaultSettings()`.
3. Write tests in `tests/XsPub.Tests/TransformationTests.cs`.

## Code conventions

- File-scoped namespaces, `Nullable=enable`, C# 12 target-typed `new()`
- `.editorconfig` is authoritative for style (based on nunit-comparisons)
- `XsPub.Library` and `XsPub.Runtime` suppress flow-analysis nullable
  warnings (CS8600–CS8625) — these are pre-nullable ported code; new code
  should be fully nullable-annotated

## Security note

`LocalOnlyXmlResolver` is the trust boundary for schema loading. Any change to
its allow/block logic requires corresponding test updates in
`LocalOnlyXmlResolverTests.cs`.
