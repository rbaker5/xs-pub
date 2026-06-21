using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Schema;

namespace XsPub.Library.Xml.Schema;

internal static class EnumConverter
{
    private static readonly char[] _whiteSpaceCharacters = new[] { ' ', '\t', '\n', '\r' };

    private static readonly XmlSchemaDerivationMethod[] _xmlSchemaDerivationMethodsFlags = new[] {
                                                                                                    XmlSchemaDerivationMethod.Extension,
                                                                                                    XmlSchemaDerivationMethod.List,
                                                                                                    XmlSchemaDerivationMethod.Restriction,
                                                                                                    XmlSchemaDerivationMethod.Substitution,
                                                                                                    XmlSchemaDerivationMethod.Union
                                                                                                };

    internal static XmlSchemaDerivationMethod ParseDerivationMethod(string value)
    {
        switch (value)
        {
            case "":
                return XmlSchemaDerivationMethod.Empty;
            case "#all":
                return XmlSchemaDerivationMethod.All;
            default:
                var derivation = parseSingleDerivationMethod(value);
                if (derivation != XmlSchemaDerivationMethod.None) return derivation;
                derivation = 0;

                foreach (var subDerivation in value.Split(_whiteSpaceCharacters, StringSplitOptions.RemoveEmptyEntries)
                                                   .Select(parseSingleDerivationMethod))
                {
                    if (subDerivation == XmlSchemaDerivationMethod.None) return XmlSchemaDerivationMethod.None;
                    derivation |= subDerivation;
                }
                return derivation;
        }
    }

    /// <exception cref="ArgumentOutOfRangeException"><c>value</c> is out of range.</exception>
    internal static string ToString(XmlSchemaDerivationMethod value)
    {
        switch (value)
        {
            case XmlSchemaDerivationMethod.Empty:
                return string.Empty;
            case XmlSchemaDerivationMethod.All:
                return "#all";
            case XmlSchemaDerivationMethod.Extension:
                return "extension";
            case XmlSchemaDerivationMethod.List:
                return "list";
            case XmlSchemaDerivationMethod.Restriction:
                return "restriction";
            case XmlSchemaDerivationMethod.Substitution:
                return "substitution";
            case XmlSchemaDerivationMethod.Union:
                return "union";
            default:
                var foundFlags = _xmlSchemaDerivationMethodsFlags.Where(flag => value.HasFlag(flag)).ToList();
                if (foundFlags.Count == 0)
                    throw new ArgumentOutOfRangeException("value", value, "Invalid enumeration value.");

                return String.Join(" ", foundFlags.Select(ToString));
        }
    }

    private static XmlSchemaDerivationMethod parseSingleDerivationMethod(string value)
    {
        switch (value)
        {
            case "extension":
                return XmlSchemaDerivationMethod.Extension;
            case "list":
                return XmlSchemaDerivationMethod.List;
            case "restriction":
                return XmlSchemaDerivationMethod.Restriction;
            case "substitution":
                return XmlSchemaDerivationMethod.Substitution;
            case "union":
                return XmlSchemaDerivationMethod.Union;
            default:
                return XmlSchemaDerivationMethod.None;
        }
    }

    internal static XmlSchemaForm ParseForm(string value)
    {
        switch (value)
        {
            case "unqualified":
                return XmlSchemaForm.Unqualified;
            case "qualified":
                return XmlSchemaForm.Qualified;
            default:
                return XmlSchemaForm.None;
        }
    }

    /// <exception cref="ArgumentOutOfRangeException"><c>value</c> is out of range.</exception>
    internal static string ToString(XmlSchemaForm value)
    {
        switch (value)
        {
            case XmlSchemaForm.Unqualified:
                return "unqualified";
            case XmlSchemaForm.Qualified:
                return "qualified";
            default:
                throw new ArgumentOutOfRangeException("value", value, "Invalid enumeration value.");
        }
    }

    internal static XmlSchemaContentProcessing ParseContentProcessing(string value)
    {
        switch (value)
        {
            case "lax":
                return XmlSchemaContentProcessing.Lax;
            case "strict":
                return XmlSchemaContentProcessing.Strict;
            case "skip":
                return XmlSchemaContentProcessing.Skip;
            default:
                return XmlSchemaContentProcessing.None;
        }
    }

    /// <exception cref="ArgumentOutOfRangeException"><c>value</c> is out of range.</exception>
    internal static string ToString(XmlSchemaContentProcessing value)
    {
        switch (value)
        {
            case XmlSchemaContentProcessing.Lax:
                return "lax";
            case XmlSchemaContentProcessing.Strict:
                return "strict";
            case XmlSchemaContentProcessing.Skip:
                return "skip";
            default:
                throw new ArgumentOutOfRangeException("value", value, "Invalid enumeration value.");
        }
    }

    internal static XmlSchemaUse ParseUse(string value)
    {
        switch (value)
        {
            case "optional":
                return XmlSchemaUse.Optional;
            case "prohibited":
                return XmlSchemaUse.Prohibited;
            case "required":
                return XmlSchemaUse.Required;
            default:
                return XmlSchemaUse.None;
        }
    }

    /// <exception cref="ArgumentOutOfRangeException"><c>value</c> is out of range.</exception>
    internal static string ToString(XmlSchemaUse value)
    {
        switch (value)
        {
            case XmlSchemaUse.Optional:
                return "optional";
            case XmlSchemaUse.Prohibited:
                return "prohibited";
            case XmlSchemaUse.Required:
                return "required";
            default:
                throw new ArgumentOutOfRangeException("value", value, "Invalid enumeration value.");
        }
    }
}
