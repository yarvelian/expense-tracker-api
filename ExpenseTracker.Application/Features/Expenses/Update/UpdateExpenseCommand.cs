using ExpenseTracker.Domain.Enums;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Update;

public sealed record UpdateExpenseCommand(
    Guid UserId,
    Guid ExpenseId,
    decimal Amount,
    ExpenseCategory ExpenseCategory,
    DateTime ExpenseDate,
    string? Description) : IRequest<Unit>;
