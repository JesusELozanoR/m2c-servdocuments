using System;
using System.Runtime.Serialization;

namespace ServDocumentos.Core.Excepciones
{
    public class ExternoTechnicalException : SystemException
    {
        public ExternoTechnicalException()
        {
        }

        public ExternoTechnicalException(string message) : base(message)
        {
        }

        public ExternoTechnicalException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ExternoTechnicalException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
