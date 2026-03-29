using ExpenseTracker.Application.Abstractions;
using ExpenseTracker.Domain.Entities;
using MediatR;

namespace ExpenseTracker.Application.Features.User.Registration;

public sealed class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserCredentialsRepository _credentialsRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserHandler(
        IUserRepository userRepository,
        IUserCredentialsRepository credentialsRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _credentialsRepository = credentialsRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(RegisterUserCommand command, CancellationToken ct)
    {
        var existingUser = await _userRepository
            .GetByEmailAsync(command.Email, ct);
        
        if (existingUser != null)
        {
            throw new InvalidOperationException("User already exists.");
        }

        var user = new Domain.Entities.User(command.Email);

        var passwordHash = _passwordHasher.Hash(command.Password);

        var credentials = new UserCredentials(user.Id, passwordHash);

        await _userRepository.AddAsync(user, ct);
        await _credentialsRepository.AddAsync(credentials, ct);

        await _unitOfWork.SaveChangesAsync(ct);

        return user.Id;
    }
}