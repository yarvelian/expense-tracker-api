using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Application.Abstractions;

public interface IExpenseRepository
{
    Task<Expense?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<Expense>> GetByUserIdAsync(Guid userId, CancellationToken ct);
    Task<IReadOnlyList<Expense>> GetFilteredAsync(
        Guid userId,
        ExpenseDateFilter filter,
        DateTime? startDate,
        DateTime? endDate,
        CancellationToken ct);
    Task AddAsync(Expense expense, CancellationToken ct);
    void Update(Expense expense);
    void Remove(Expense expense);
}
