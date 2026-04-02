using ExpenseTracker.Application.Abstractions;
using ExpenseTracker.Domain.Entities;
using MediatR;

namespace ExpenseTracker.Application.Features.User.Login;

public sealed class LoginHandler : IRequestHandler<LoginCommand, string>
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly IUserCredentialsRepository _credentialsRepository;
    
    public LoginHandler(
        IPasswordHasher passwordHasher,
        IUserCredentialsRepository credentialsRepository,
        IJwtService jwtService)
    {
        _passwordHasher = passwordHasher;
        _credentialsRepository  = credentialsRepository;
        _jwtService = jwtService;
    }

    public async Task<string> Handle(LoginCommand command, CancellationToken ct)
    {
        var email = command.Email.ToLowerInvariant();
        
        var userCredentials = await _credentialsRepository.GetByEmailAsync(email, ct);
        
        if (userCredentials == null)
        {
            throw new InvalidOperationException("Invalid Credentials");
        }
        
        var isPasswordVerified = _passwordHasher.Verify(command.Password, userCredentials.PasswordHash);
        
        if (!isPasswordVerified)
        {
            throw new InvalidOperationException("Invalid Credentials");
        }
        
        var jwtToken = _jwtService.GenerateToken(userCredentials.UserId, email);
        
        return jwtToken;
    }
}