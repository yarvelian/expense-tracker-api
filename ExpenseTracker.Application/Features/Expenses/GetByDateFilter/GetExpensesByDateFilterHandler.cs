using ExpenseTracker.Application.Abstractions;
using ExpenseTracker.Application.Features.Expenses.Shared;
using ExpenseTracker.Domain.Entities;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.GetByDateFilter;

public sealed class GetExpensesByDateFilterHandler
    : IRequestHandler<GetExpensesByDateFilterQuery, IReadOnlyList<ExpenseDto>>
{
    private readonly IExpenseRepository _expenseRepository;

    public GetExpensesByDateFilterHandler(IExpenseRepository expenseRepository)
    {
        _expenseRepository  = expenseRepository;
    }
    
    public async Task<IReadOnlyList<ExpenseDto>> Handle(GetExpensesByDateFilterQuery query, CancellationToken ct)
    {
        IReadOnlyList<Expense> expenses = await _expenseRepository.GetFilteredAsync(
            query.UserId,
            query.Filter,
            query.StartDate,
            query.EndDate,
            ct);

        IReadOnlyList<ExpenseDto> expenseDtos = expenses.Select(e =>
            new ExpenseDto(
                e.Id,
                e.UserId,
                e.Amount,
                e.Category,
                e.ExpenseDate,
                e.Description
            )).ToList();

        return expenseDtos;
    }
}
