using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Delete;

public sealed record DeleteExpenseCommand(Guid UserId, Guid ExpenseId) : IRequest<Unit>;
