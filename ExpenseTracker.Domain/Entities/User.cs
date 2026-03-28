using ExpenseTracker.Domain.Common;

namespace ExpenseTracker.Domain.Entities;

public class User : Entity
{
    public string Email { get; private set; }
    
    private  User() {
    }

    public User(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) 
        {
            throw new ArgumentException("Email is required.");
        }
        
        Id = Guid.NewGuid();
        Email = email.ToLowerInvariant();
        CreatedAtUtc = DateTime.UtcNow;
    }
}