namespace ExpenseTracker.Domain.Common;

public abstract class Entity
{
    public Guid Id { get; protected  set; }
    public DateTime CreatedAtUtc { get; protected set; }
}
