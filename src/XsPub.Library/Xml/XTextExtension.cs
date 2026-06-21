using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace XsPub.Library.Xml;

public static class XTextExtension
{
    public static int GetFirstLineIndentation(this XText text)
    {
        return text.Value.TrimStart('\n', '\r').Length -
               text.Value.TrimStart('\n', '\r', ' ').Length;
    }

    public static string GetFirstLineIndentationText(this XText text)
    {
        return text.GetFirstLineIndentationText(' ', '\t');
    }

    public static string GetFirstLineIndentationText(this XText text, params char[] indentationCharacters)
    {
        var newLineCharacters = new[] {'\n', '\r'};
        var fullLength = text.Value.Length;
        var fromFirstLineLength = text.Value.TrimStart(newLineCharacters).Length;
        var fromFirstNonWhiteSpaceLength = text.Value.TrimStart(indentationCharacters.Union(newLineCharacters).ToArray()).Length;
        return text.Value.Substring(fullLength - fromFirstLineLength,
                                    fromFirstLineLength - fromFirstNonWhiteSpaceLength);
    }
}