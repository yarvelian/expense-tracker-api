using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Api.Models;

public sealed record AddExpenseRequest(decimal Amount,
    ExpenseCategory ExpenseCategory,
    DateTime ExpenseDate,
    string? Description);
