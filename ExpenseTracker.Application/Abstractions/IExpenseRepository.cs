using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Application.Abstractions;

public interface IExpenseRepository
{
    Task<Expense?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<Expense>> GetByUserIdAsync(Guid userId, CancellationToken ct);
    Task AddAsync(Expense expense, CancellationToken ct);
    void Update(Expense expense);
    void Remove(Expense expense);
}
