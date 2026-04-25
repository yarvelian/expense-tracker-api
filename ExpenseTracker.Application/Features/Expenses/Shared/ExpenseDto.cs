using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Application.Features.Expenses.Shared;

public sealed record ExpenseDto(
    Guid Id, 
    Guid UserId, 
    decimal Amount,
    ExpenseCategory ExpenseCategory, 
    DateTime ExpenseDate, 
    string? Description);
