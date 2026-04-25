using FluentValidation;

namespace ExpenseTracker.Application.Features.Expenses.Delete;

public sealed class DeleteExpenseCommandValidator : AbstractValidator<DeleteExpenseCommand>
{
    public DeleteExpenseCommandValidator()
    {
        RuleFor(command => command.UserId)
            .NotEmpty();
        
        RuleFor(command => command.ExpenseId)
            .NotEmpty();
    }
}
