using System;
using System.Runtime.Serialization;

namespace Pepegov.UnitOfWork.Exceptions;

[Serializable]
public class UnitOfWorkConfigurationException : Exception
{
    public UnitOfWorkConfigurationException() { }
    public UnitOfWorkConfigurationException(string message) : base(message) { }
    public UnitOfWorkConfigurationException(string message, Exception innerException) : base(message, innerException) { }
    protected UnitOfWorkConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context) { }    

}