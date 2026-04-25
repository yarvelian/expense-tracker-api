using ExpenseTracker.Application.Features.Expenses.Shared;
using ExpenseTracker.Domain.Enums;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.GetByDateFilter;

public sealed record GetExpensesByDateFilterQuery(
    Guid UserId,
    ExpenseDateFilter Filter,
    DateTime? StartDate,
    DateTime? EndDate) : IRequest<IReadOnlyList<ExpenseDto>>;
