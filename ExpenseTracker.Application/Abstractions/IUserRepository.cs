using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Application.Abstractions;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct);
    Task AddAsync(User user, CancellationToken ct);
}
