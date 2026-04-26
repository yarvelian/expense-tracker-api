using ExpenseTracker.Application.Abstractions;
using ExpenseTracker.Application.Features.User.Login;
using ExpenseTracker.Domain.Entities;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace ExpenseTracker.Application.Tests.Features.User;

public sealed class LoginUserHandlerTests
{
    private readonly IUserCredentialsRepository _credentialsRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    private readonly LoginHandler _handler;

    public LoginUserHandlerTests()
    {
        _credentialsRepository = Substitute.For<IUserCredentialsRepository>();
        _passwordHasher = Substitute.For<IPasswordHasher>();
        _jwtService = Substitute.For<IJwtService>();

        _handler = new LoginHandler(
            _passwordHasher,
            _credentialsRepository,
            _jwtService
        );
    }

    [Fact]
    public async Task Handle_WhenCredentialsAreValid_ShouldReturnToken()
    {
        var command = new LoginCommand("test@example.com", "password123");
        var userCredentials = new UserCredentials(Guid.NewGuid(), "password_hash");

        _credentialsRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>())
            .Returns(userCredentials);

        _passwordHasher.Verify(command.Password, userCredentials.PasswordHash)
            .Returns(true);

        _jwtService.GenerateToken(userCredentials.UserId, command.Email)
            .Returns("jwt_token");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be("jwt_token");
        _jwtService.Received(1).GenerateToken(userCredentials.UserId, command.Email);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ShouldThrowInvalidOperationException()
    {
        var command = new LoginCommand("unknown@example.com", "password123");

        _credentialsRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>())
            .ReturnsNull();

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Invalid Credentials");
    }

    [Fact]
    public async Task Handle_WhenPasswordIsWrong_ShouldThrowInvalidOperationException()
    {
        var command = new LoginCommand("test@example.com", "wrong_password");
        var userCredentials = new UserCredentials(Guid.NewGuid(), "password_hash");

        _credentialsRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>())
            .Returns(userCredentials);

        _passwordHasher.Verify(command.Password, userCredentials.PasswordHash)
            .Returns(false);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Invalid Credentials");
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ShouldNotCallGenerateToken()
    {
        var command = new LoginCommand("unknown@example.com", "password123");

        _credentialsRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>())
            .ReturnsNull();

        var act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>();

        _jwtService.DidNotReceive().GenerateToken(Arg.Any<Guid>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Handle_WhenPasswordIsWrong_ShouldNotCallGenerateToken()
    {
        var command = new LoginCommand("test@example.com", "wrong_password");
        var userCredentials = new UserCredentials(Guid.NewGuid(), "password_hash");

        _credentialsRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>())
            .Returns(userCredentials);

        _passwordHasher.Verify(command.Password, userCredentials.PasswordHash)
            .Returns(false);

        var act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>();

        _jwtService.DidNotReceive().GenerateToken(Arg.Any<Guid>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Handle_WhenEmailHasMixedCase_ShouldNormalizeAndReturnToken()
    {
        const string mixedCaseEmail = "Test@Example.COM";
        const string normalizedEmail = "test@example.com";
        var command = new LoginCommand(mixedCaseEmail, "password123");
        var userCredentials = new UserCredentials(Guid.NewGuid(), "password_hash");

        _credentialsRepository.GetByEmailAsync(normalizedEmail, Arg.Any<CancellationToken>())
            .Returns(userCredentials);

        _passwordHasher.Verify(command.Password, userCredentials.PasswordHash)
            .Returns(true);

        _jwtService.GenerateToken(userCredentials.UserId, normalizedEmail)
            .Returns("jwt_token");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be("jwt_token");
        await _credentialsRepository.Received(1).GetByEmailAsync(normalizedEmail, Arg.Any<CancellationToken>());
        _jwtService.Received(1).GenerateToken(userCredentials.UserId, normalizedEmail);
    }
}
