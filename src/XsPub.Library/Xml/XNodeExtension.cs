using System.Xml;
using System.Xml.Linq;

namespace XsPub.Library.Xml;

public static class XNodeExtension
{
    public static int GetSpaceIndentation(this XNode element)
    {
        return element.GetIndentationText(' ').Length;
    }

    public static string GetIndentationText(this XNode element)
    {
        return element.GetIndentationText(' ', '\t');
    }

    /// <summary>
    /// Return the characters that are between the current node and the beginning of the line.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="indentationCharacters"></param>
    /// <returns></returns>
    public static string GetIndentationText(this XNode node, params char[] indentationCharacters)
    {
        // TODO: Requires some more thought about handling text nodes themselves.
        if (node.PreviousNode == null)
            return string.Empty;

        if (node.PreviousNode.NodeType != XmlNodeType.Text)
            return string.Empty;

        var nodeSpace = node.PreviousNode.ToString();
        var nodeSpaceLength = nodeSpace.Length;
        var trailingIndentationLength = nodeSpaceLength - nodeSpace.TrimEnd(indentationCharacters).Length;
        // Get all characters at the end of the indentation.
        return nodeSpace.Substring(nodeSpaceLength - trailingIndentationLength);
    }
}