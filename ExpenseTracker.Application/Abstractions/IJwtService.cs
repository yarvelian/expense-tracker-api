namespace ExpenseTracker.Application.Abstractions;

public interface IJwtService
{
    string GenerateToken(Guid userId, string email);
}