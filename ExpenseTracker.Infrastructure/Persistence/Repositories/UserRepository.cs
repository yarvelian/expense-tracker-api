using ExpenseTracker.Application.Abstractions;
using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infrastructure.Persistence.Repositories;

internal sealed class UserRepository: IUserRepository
{
     
    private readonly ApplicationDbContext _dbContext;

    public UserRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public Task<User?> GetByEmailAsync(string email, CancellationToken ct)
    {
        return _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), ct);
    }
    
    public Task AddAsync(User user, CancellationToken ct)
    {
        _dbContext.Users.Add(user);
        return Task.CompletedTask;
    }
}