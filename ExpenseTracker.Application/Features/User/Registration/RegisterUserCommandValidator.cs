using FluentValidation;

namespace ExpenseTracker.Application.Features.User.Registration;

public sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public  RegisterUserCommandValidator()
    {
        RuleFor(user => user.Email)
            .NotEmpty()
            .EmailAddress();
        RuleFor(user => user.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(64);
    }
}
