using System;
using System.Runtime.Serialization;

namespace Pepegov.UnitOfWork.Exceptions
{
    /// <summary>
    /// UnitOfWork Argument Null Exception
    /// </summary>
    [Serializable]
    public class UnitOfWorkArgumentNullException : ArgumentNullException
    {
        public UnitOfWorkArgumentNullException() { }
        public UnitOfWorkArgumentNullException(string message) : base(message) { }
        public UnitOfWorkArgumentNullException(string message, Exception innerException) : base(message, innerException) { }
        protected UnitOfWorkArgumentNullException(SerializationInfo info, StreamingContext context) : base(info, context) { }    
    }
}
