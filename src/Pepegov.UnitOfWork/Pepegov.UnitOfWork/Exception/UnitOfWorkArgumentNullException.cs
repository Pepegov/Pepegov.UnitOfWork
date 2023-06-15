namespace Pepegov.UnitOfWork.Exception
{
    /// <summary>
    /// UnitOfWork Argument Null Exception
    /// </summary>
    public class UnitOfWorkArgumentNullException : System.Exception
    {
        public UnitOfWorkArgumentNullException(string? message) : base(message) { }

        public UnitOfWorkArgumentNullException(string? message, System.Exception exception) : base(message, exception) { }
    }
}
