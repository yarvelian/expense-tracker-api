using ExpenseTracker.Domain.Common;
using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Domain.Entities;

public class Expense : Entity
{
    public Guid UserId { get; private set; }
    public decimal Amount { get; private set; }
    public ExpenseCategory Category { get; private set; }
    public DateTime ExpenseDate { get; private set; }
    
    public string? Description { get; private set; }
    
    private Expense() { }

    public Expense(
        Guid userId,
        decimal amount,
        ExpenseCategory category,
        DateTime expenseDate,
        string? description)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("UserId is required.");
        }

        if (amount <= 0)
        {
            throw new ArgumentException("Amount must be greater than zero.");
        }

        if (expenseDate == default)
        {
            throw new ArgumentException("ExpenseDate is required.");
        }

        Id = Guid.NewGuid();
        UserId = userId;
        Amount = amount;
        Category = category;
        ExpenseDate = expenseDate;
        Description = string.IsNullOrWhiteSpace(description)
            ? null
            : description.Trim();

        CreatedAtUtc = DateTime.UtcNow;
    }
}
