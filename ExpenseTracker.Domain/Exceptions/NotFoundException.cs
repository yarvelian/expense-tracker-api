namespace ExpenseTracker.Domain.Exceptions;

public sealed class NotFoundException : Exception
{
    public NotFoundException()
    {
    }
    
    public NotFoundException(string message) : base(message)
    {
    }
}
