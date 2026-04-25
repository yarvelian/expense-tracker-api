using ExpenseTracker.Application.Abstractions;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infrastructure.Persistence.Repositories;

public sealed class ExpenseRepository : IExpenseRepository
{
    private readonly ApplicationDbContext _dbContext;
    
    public ExpenseRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Expense?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return _dbContext.Expenses.FirstOrDefaultAsync(expense => expense.Id == id, ct);
    }

    public async Task<IReadOnlyList<Expense>> GetByUserIdAsync(Guid userId, CancellationToken ct)
    {
        return await _dbContext.Expenses.Where(expense => expense.UserId == userId).ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Expense>> GetFilteredAsync(
        Guid userId,
        ExpenseDateFilter filter,
        DateTime? startDate,
        DateTime? endDate,
        CancellationToken ct)
    {
        IQueryable<Expense> expenses = _dbContext.Expenses.Where(expense => expense.UserId == userId);
        var (start, end) = filter switch
        {
            ExpenseDateFilter.Custom => (startDate!.Value, endDate!.Value),
            ExpenseDateFilter.PastWeek =>
            (
                DateTime.UtcNow.AddDays(-7),
                DateTime.UtcNow
            ),
            ExpenseDateFilter.PastMonth =>
            (
                DateTime.UtcNow.AddMonths(-1),
                DateTime.UtcNow
            ),
            ExpenseDateFilter.LastThreeMonths =>
            (
                DateTime.UtcNow.AddMonths(-3),
                DateTime.UtcNow
            ),
            _ => throw new ArgumentOutOfRangeException(nameof(filter), filter, "Unsupported expense date filter.")
        };
        expenses = expenses
            .Where(exp => exp.ExpenseDate >= start && exp.ExpenseDate <= end)
            .OrderByDescending(exp => exp.ExpenseDate); 
        
        return await expenses.ToListAsync(ct);
    }

    public Task AddAsync(Expense expense, CancellationToken ct)
    {
        _dbContext.Expenses.Add(expense);
        return Task.CompletedTask;
    }

    public void Update(Expense expense)
    {
        _dbContext.Expenses.Update(expense);
    }

    public void Remove(Expense expense)
    {
        _dbContext.Expenses.Remove(expense);
    }
}
