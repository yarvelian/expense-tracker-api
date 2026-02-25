namespace ExpenseTracker.Application.Features.User.Registration;

public sealed record RegisterUserCommand(
    string Email,
    string Password
);