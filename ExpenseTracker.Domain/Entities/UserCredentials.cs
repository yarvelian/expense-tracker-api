using ExpenseTracker.Domain.Common;

namespace ExpenseTracker.Domain.Entities;

public class UserCredentials : Entity
{
    public Guid UserId { get; private set; }
    public string PasswordHash { get; private set; } = null!;

    private UserCredentials()
    {
    }
    
    public UserCredentials(Guid userId, string passwordHash)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("UserId is required.");
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException("PasswordHash is required.");
        }
        
        Id = Guid.NewGuid();
        UserId = userId;
        PasswordHash = passwordHash;
        CreatedAtUtc = DateTime.UtcNow;
    }
}
