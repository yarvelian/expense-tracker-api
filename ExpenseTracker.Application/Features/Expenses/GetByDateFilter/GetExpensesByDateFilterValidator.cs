using ExpenseTracker.Domain.Enums;
using FluentValidation;

namespace ExpenseTracker.Application.Features.Expenses.GetByDateFilter;

public sealed class GetExpensesByDateFilterValidator : AbstractValidator<GetExpensesByDateFilterQuery>
{
    public GetExpensesByDateFilterValidator()
    {
        RuleFor(command => command.UserId)
            .NotEmpty();

        When(query => query.Filter == ExpenseDateFilter.Custom, () =>
        {
            RuleFor(query => query.EndDate)
                .NotNull()
                .WithMessage("EndDate is required when using Custom date filter");

            RuleFor(query => query.StartDate)
                .NotNull()
                .WithMessage("StartDate is required when using Custom date filter")
                .DependentRules(() =>
                {
                    RuleFor(query => query)
                        .Must(query => !query.EndDate.HasValue || query.StartDate <= query.EndDate)
                        .WithMessage("StartDate cannot be after EndDate");
                });
        });
    }
}
