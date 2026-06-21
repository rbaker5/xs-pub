using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace XsPub.Library.Xml.Schema;

[Serializable]
public class XsException : Exception
{
    private const string DefaultMessage = "A schema error occurred.";

    public int LineNumber { get; private set; }
    public int LinePosition { get; private set; }
    public XsObject SourceSchemaObject { get; private set; }
    public string SourceUri { get; private set; }

    public XsException()
        : this(null)
    {
    }

    public XsException(string message)
        : this(message, (Exception)null, 0, 0)
    {
    }

    protected XsException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        SourceUri = (string)info.GetValue("sourceUri", typeof(string));
        LineNumber = (int)info.GetValue("lineNumber", typeof(int));
        LinePosition = (int)info.GetValue("linePosition", typeof(int));
    }

    public XsException(string message, Exception innerException)
        : this(message, innerException, 0, 0)
    {
    }

    internal XsException(string message, XsObject source)
        : this(message, null, source.SourceUri, source.LineNumber, source.LinePosition, source)
    {
    }

    internal XsException(string message, XsObject source, Exception innerException)
        : this(message, innerException, source.SourceUri, source.LineNumber, source.LinePosition, source)
    {
    }

    public XsException(string message, Exception innerException, int lineNumber, int linePosition)
        : this(message, innerException, null, lineNumber, linePosition, null)
    {
    }

    internal XsException(string message, string sourceUri, int lineNumber, int linePosition)
        : this(message, null, sourceUri, lineNumber, linePosition, null)
    {
    }

    private XsException(string message, Exception innerException, string sourceUri, int lineNumber, int linePosition, XsObject source)
        : base(message ?? DefaultMessage, innerException)
    {
        SourceUri = sourceUri;
        LineNumber = lineNumber;
        LinePosition = linePosition;
        SourceSchemaObject = source;
    }

    [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue("sourceUri", SourceUri);
        info.AddValue("lineNumber", LineNumber);
        info.AddValue("linePosition", LinePosition);
    }

    internal void SetSchemaObject(XsObject source)
    {
        SourceSchemaObject = source;
    }

    internal void SetSource(XsObject source)
    {
        SourceSchemaObject = source;
        SourceUri = source.SourceUri;
        LineNumber = source.LineNumber;
        LinePosition = source.LinePosition;
    }

    internal void SetSource(string sourceUri, int lineNumber, int linePosition)
    {
        SourceUri = sourceUri;
        LineNumber = lineNumber;
        LinePosition = linePosition;
    }
}

