using FluentValidation;

namespace ExpenseTracker.Application.Features.Expenses.Update;

public sealed class UpdateExpenseCommandValidator : AbstractValidator<UpdateExpenseCommand>
{
    public  UpdateExpenseCommandValidator()
    {
        RuleFor(command => command.UserId)
            .NotEmpty();
        
        RuleFor(command => command.ExpenseId)
            .NotEmpty();
        
        RuleFor(command => command.ExpenseCategory)
            .IsInEnum()
            .WithMessage("Please specify a valid expense category");
        
        RuleFor(command => command.ExpenseDate)
            .NotEmpty()
            .WithMessage("Please specify an expense date");
        
        RuleFor(command => command.Amount)
            .GreaterThan(0)
            .WithMessage("Please specify an amount");

        RuleFor(command => command.Amount)
            .PrecisionScale(18, 2, true);
        
        RuleFor(command => command.Description)
            .MaximumLength(600)
            .WithMessage("Description must not be longer than 600 characters");
    }
}
