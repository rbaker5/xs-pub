using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace XsPub.Library.Xml;

/// <summary>
/// Extension methods to assist formatting and manipulating containers.
/// </summary>
public static class XContainerExtension
{
    private const string DefaultIndentationString = "  ";

    /// <summary>
    /// Places all child elements on a new line, indents each with two additional spaces ("  ")
    /// and insures the end tag is consistently indented.
    /// </summary>
    /// <param name="element">
    /// The element to change.  The indentation of this element will not change.
    /// </param>
    public static void Reformat(this XContainer element)
    {
        element.Reformat(DefaultIndentationString);
    }

    /// <summary>
    /// Places all child elements on a new line, indents each according to 
    /// <paramref name="nodeIndentationString"/> and insures the end tag is consistently indented.
    /// </summary>
    /// <param name="element">
    /// The element to change.  The indentation of this element will not change.
    /// </param>
    /// <param name="nodeIndentationString">
    /// A string with the characters (tabs or spaces for example) that are the difference between
    /// one node's indentation and it's children's indentation.
    /// </param>
    public static void Reformat(this XContainer element, string nodeIndentationString)
    {
        var nodeIndentation = element.GetIndentationText();
        foreach (var node in element.Nodes().ToList())
        {
            if (node.NodeType == XmlNodeType.Text)
            {
                if (String.IsNullOrEmpty(node.ToString().Trim(' ', '\r', '\n', '\t')))
                    node.Remove();
                else if (node is XText text)
                {
                    var firstLineIndentation = "\n" + text.GetFirstLineIndentationText();
                    var newIndentation = "\n" + nodeIndentation + nodeIndentationString;
                    
                    text.Value = text.Value.Replace(firstLineIndentation, newIndentation)
                        .TrimEnd(' ', '\n', '\r', '\t');
                }
            }
            else
            {
                node.AddBeforeSelf(Environment.NewLine + nodeIndentation + nodeIndentationString);
                var childElement = node as XElement;
                if (childElement != null)
                    childElement.Reformat(nodeIndentationString);
            }
        }
        if (element.LastNode != null)
            element.LastNode.AddAfterSelf(Environment.NewLine + nodeIndentation);
    }

    /// <summary>
    /// Takes a list of elements (assumes the elements are siblings) and insures that each 
    /// element is on a new line and has identical indentation to the first element and all 
    /// children are further indented by two spaces ("  ") per level.
    /// </summary>
    /// <param name="siblings">The list of siblings to indent.</param>
    public static void Reformat(this IEnumerable<XElement> siblings)
    {
        siblings.Reformat(DefaultIndentationString);
    }

    /// <summary>
    /// Takes a list of elements (assumes the elements are siblings) and insures that each 
    /// element is on a new line and has identical indentation to the first element and all 
    /// children are further indented by <paramref name="nodeIndentationString"/>.
    /// </summary>
    /// <param name="siblings">A list of siblings to indent.</param>
    /// <param name="nodeIndentationString">
    /// A string with the characters (tabs or spaces for example) that are the difference between
    /// one node's indentation and it's children's indentation.
    /// </param>
    public static void Reformat(this IEnumerable<XElement> siblings, string nodeIndentationString)
    {
        // For Future Contract Rule
        // siblings.All(sibling => sibling.Parent == siblings.First().Parent);

        var nodeList = siblings.ToList();
        var nodeIndentation = siblings.OfType<XElement>().First().GetIndentationText();
        foreach (var childElement in nodeList)
        {
            childElement.AddBeforeSelf(Environment.NewLine + nodeIndentation);
            childElement.Reformat(nodeIndentationString);
        }
        if (nodeList.Last() != null)
            nodeList.Last().AddAfterSelf(Environment.NewLine + nodeIndentation);
    }
}