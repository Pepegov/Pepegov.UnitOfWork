using System.Runtime.Serialization;

namespace Pepegov.UnitOfWork.Exceptions;

[Serializable]
public class UnitOfWorkTransactionNotSupported : Exception
{
    public UnitOfWorkTransactionNotSupported() { }
    public UnitOfWorkTransactionNotSupported(string message) : base(message) { }
    public UnitOfWorkTransactionNotSupported(string message, Exception innerException) : base(message, innerException) { }
    protected UnitOfWorkTransactionNotSupported(SerializationInfo info, StreamingContext context) : base(info, context) { }    
}