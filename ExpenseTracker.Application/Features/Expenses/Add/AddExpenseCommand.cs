using ExpenseTracker.Domain.Enums;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Add;

public sealed record AddExpenseCommand(
    Guid UserId,
    decimal Amount,
    ExpenseCategory ExpenseCategory,
    DateTime ExpenseDate,
    string? Description) : IRequest<Guid>;
