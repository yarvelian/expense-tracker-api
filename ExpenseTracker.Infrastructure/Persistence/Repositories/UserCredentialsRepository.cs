using ExpenseTracker.Application.Abstractions;
using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infrastructure.Persistence.Repositories;

internal sealed class UserCredentialsRepository : IUserCredentialsRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UserCredentialsRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<UserCredentials?> GetByUserIdAsync(Guid userId, CancellationToken ct)
    {
        return _dbContext.UserCredentials.FirstOrDefaultAsync(uc => uc.UserId == userId, ct);
    }

    public Task<UserCredentials?> GetByEmailAsync(string email, CancellationToken ct)
    {
        var credentials = from cred in _dbContext.UserCredentials
            join user in _dbContext.Users on cred.UserId equals user.Id
            where user.Email == email.ToLowerInvariant()
            select cred;
        
        return credentials.FirstOrDefaultAsync(ct);
    }

    public Task AddAsync(UserCredentials credentials, CancellationToken ct)
    {
        _dbContext.UserCredentials.Add(credentials);
        return Task.CompletedTask;
    }
}