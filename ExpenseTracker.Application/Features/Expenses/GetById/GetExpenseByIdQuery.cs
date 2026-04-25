using ExpenseTracker.Application.Features.Expenses.Shared;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.GetById;

public sealed record GetExpenseByIdQuery(Guid UserId, Guid ExpenseId) : IRequest<ExpenseDto>;
