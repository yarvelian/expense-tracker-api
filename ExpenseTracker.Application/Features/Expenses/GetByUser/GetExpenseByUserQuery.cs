using ExpenseTracker.Application.Features.Expenses.Shared;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.GetByUser;

public sealed record GetExpenseByUserQuery(Guid UserId) : IRequest<IReadOnlyList<ExpenseDto>>;
