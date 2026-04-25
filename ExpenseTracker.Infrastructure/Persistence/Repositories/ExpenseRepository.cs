using ExpenseTracker.Application.Abstractions;
using ExpenseTracker.Domain.Entities;
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
