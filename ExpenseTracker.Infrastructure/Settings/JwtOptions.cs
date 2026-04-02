using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Infrastructure.Settings;

public sealed class JwtOptions
{
    [Required]
    public string SecretKey { get; init; } = null!;
    [Required]
    public string Issuer { get; init; } = null!;
    [Required]
    public string Audience { get; init; } = null!;
    [Range(1, 1440)]
    public int ExpirationMinutes { get; init; }
}
