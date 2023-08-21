using System;
using System.Collections.Generic;

namespace Pepegov.UnitOfWork.Entityes;

public sealed class SaveChangesResult
{
    public SaveChangesResult() => Messages = new List<string>();

    public SaveChangesResult(string message) : this() => AddMessage(message);

    public Exception? Exception { get; set; }

    public bool IsOk => Exception == null;

    public void AddMessage(string message) => Messages.Add(message);

    private List<string> Messages { get; }
}