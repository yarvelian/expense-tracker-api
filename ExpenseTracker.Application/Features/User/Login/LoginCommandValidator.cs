using FluentValidation;

namespace ExpenseTracker.Application.Features.User.Login;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(loginCommand => loginCommand.Email)
            .NotEmpty()
            .EmailAddress();
        RuleFor(loginCommand => loginCommand.Password)
            .NotEmpty();
    }
}
