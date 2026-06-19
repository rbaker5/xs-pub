# Contributing to XsPub

Thanks for your interest in contributing. This guide covers the basics.

## Before You Start

- For bug fixes and small improvements, open an issue first to discuss the change.
- For larger features, open an issue to agree on scope before writing code.
- Check existing [issues](https://github.com/rbaker5/xs-pub/issues) to avoid duplicating effort.

## Development Setup

Requires the [.NET 10 SDK](https://dot.net).

```
git clone https://github.com/rbaker5/xs-pub.git
cd xs-pub
dotnet build
dotnet test
```

## Project Layout

```
src/
  XsPub.Library/   XSD/WSDL schema object model (no external dependencies)
  XsPub.Runtime/   Transformation engine; depends on Library
  Xsp/             CLI entry point; depends on Runtime
tests/
  XsPub.Library.Tests/  NUnit suite — schema model read/compare tests
  XsPub.Tests/          xUnit suite — property round-trips, transformations, CLI
```

## Making Changes

1. Fork the repository and create a branch from `main`.
2. Write code. Run `dotnet build` and `dotnet test` before committing.
3. Add or update tests. The CI requires all tests to pass.
4. Keep commits focused — one logical change per commit.

### Adding a transformation

1. Implement `ITransformation` in `XsPub.Runtime/Transformations/`.
2. Implement `ITransformationFactory` (extend `TransformationFactoryBase`) for it.
3. Register the factory in `Xsp/Program.cs`.
4. Add at least one integration test in `XsPub.Tests/TransformationTests.cs`.

## Pull Request Checklist

- [ ] `dotnet test` passes locally
- [ ] New behaviour is covered by a test
- [ ] The PR description explains *what* changed and *why*
- [ ] Commit messages are clear and present-tense ("Add X", not "Added X")

## Code Style

- Follow existing conventions; the codebase uses C# 12 features where they aid clarity.
- No new `// TODO` comments — open an issue instead.
- Comments explain *why*, not *what*.

## Reporting Security Issues

See [SECURITY.md](.github/SECURITY.md). Do not open a public issue for vulnerabilities.
