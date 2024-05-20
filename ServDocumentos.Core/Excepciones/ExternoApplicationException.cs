using System;
using System.Runtime.Serialization;

namespace ServDocumentos.Core.Excepciones
{
    public class ExternoApplicationException : ApplicationException
    {
        public ExternoApplicationException()
        {
        }

        public ExternoApplicationException(string message) : base(message)
        {
        }

        public ExternoApplicationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ExternoApplicationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
