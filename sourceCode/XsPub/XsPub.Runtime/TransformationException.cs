using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace XsPub.Runtime
{
    public class TransformationException : Exception
    {
        public TransformationException()
        {
        }

        public TransformationException(string message) : base(message)
        {
        }

        public TransformationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TransformationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
