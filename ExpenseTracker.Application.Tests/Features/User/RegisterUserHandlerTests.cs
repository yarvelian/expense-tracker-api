using ExpenseTracker.Application.Abstractions;
using ExpenseTracker.Application.Features.User.Registration;
using ExpenseTracker.Domain.Entities;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace ExpenseTracker.Application.Tests.Features.User;

public sealed class RegisterUserHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IUserCredentialsRepository _credentialsRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;
    private readonly RegisterUserHandler _handler;

    public RegisterUserHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _credentialsRepository = Substitute.For<IUserCredentialsRepository>();
        _passwordHasher = Substitute.For<IPasswordHasher>();
        _unitOfWork = Substitute.For<IUnitOfWork>();

        _handler = new RegisterUserHandler(
            _userRepository,
            _credentialsRepository,
            _passwordHasher,
            _unitOfWork);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ShouldReturnNewUserId()
    {
        var command = new RegisterUserCommand("test@example.com", "password123");

        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>())
            .ReturnsNull();

        _passwordHasher.Hash(command.Password)
            .Returns("hashed_password");
        
        var result = await _handler.Handle(command, CancellationToken.None);
        
        result.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ShouldAddUserToRepository()
    {
        var command = new RegisterUserCommand("Test@Example.COM", "password123");

        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>())
            .ReturnsNull();

        _passwordHasher.Hash(command.Password)
            .Returns("hashed_password");

        await _handler.Handle(command, CancellationToken.None);

        await _userRepository.Received(1)
            .AddAsync(Arg.Is<Domain.Entities.User>(u => u.Email == command.Email.ToLowerInvariant()),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ShouldAddCredentialsToRepository()
    {
        var command = new RegisterUserCommand("test@example.com", "password123");
        const string hashedPassword = "hashed_password";
        var capturedUserId = Guid.Empty;

        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>())
            .ReturnsNull();

        _passwordHasher.Hash(command.Password)
            .Returns(hashedPassword);

        await _userRepository.AddAsync(
            Arg.Do<Domain.Entities.User>(u => capturedUserId = u.Id),
            Arg.Any<CancellationToken>());

        await _handler.Handle(command, CancellationToken.None);

        await _credentialsRepository.Received(1)
            .AddAsync(Arg.Is<UserCredentials>(c => c.PasswordHash == hashedPassword
                                                   && c.UserId == capturedUserId),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ShouldSaveChanges()
    {
        var command = new RegisterUserCommand("test@example.com", "password123");

        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>())
            .ReturnsNull();

        _passwordHasher.Hash(command.Password)
            .Returns("hashed_password");
        
        await _handler.Handle(command, CancellationToken.None);
        
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenUserAlreadyExists_ShouldThrowInvalidOperationException()
    {
        var command = new RegisterUserCommand("existing@example.com", "password123");
        var existingUser = new Domain.Entities.User("existing@example.com");

        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>())
            .Returns(existingUser);
        
        var act = async () => await _handler.Handle(command, CancellationToken.None);
        
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("User already exists.");
    }

    [Fact]
    public async Task Handle_WhenUserAlreadyExists_ShouldNotSaveChanges()
    {
        var command = new RegisterUserCommand("existing@example.com", "password123");
        var existingUser = new Domain.Entities.User("existing@example.com");

        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>())
            .Returns(existingUser);
        
        var act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>();
        
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
