using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Application.Abstractions;

public interface IUserCredentialsRepository
{
    Task<UserCredentials?> GetByUserIdAsync(Guid userId, CancellationToken ct);
    Task<UserCredentials?> GetByEmailAsync(string email, CancellationToken ct);
    Task AddAsync(UserCredentials credentials, CancellationToken ct);
}
