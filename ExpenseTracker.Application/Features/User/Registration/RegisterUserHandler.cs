using ExpenseTracker.Application.Abstractions;
using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Application.Features.User.Registration;

public sealed class RegisterUserHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IUserCredentialsRepository _credentialsRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserHandler(
        IUserRepository userRepository,
        IUserCredentialsRepository credentialsRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _credentialsRepository = credentialsRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Guid> Handle(RegisterUserCommand command, CancellationToken ct)
    {
        var existingUser = await _userRepository
            .GetByEmailAsync(command.Email, ct);
        
        //TODO: Also add separate transport-level validator for email length, format etc.
        if (existingUser != null)
        {
            throw new InvalidOperationException("User already exists.");
        }

        var user = new Domain.Entities.User(command.Email);

        var passwordHash = _passwordHasher.Hash(command.Password);

        var credentials = new UserCredentials(user.Id, passwordHash);

        await _userRepository.AddAsync(user, ct);
        await _credentialsRepository.AddAsync(credentials, ct);

        return user.Id;
    }
}