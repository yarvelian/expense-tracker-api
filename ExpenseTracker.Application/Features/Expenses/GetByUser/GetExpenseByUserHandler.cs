using ExpenseTracker.Application.Abstractions;
using ExpenseTracker.Application.Features.Expenses.Shared;
using ExpenseTracker.Domain.Entities;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.GetByUser;

public sealed class GetExpenseByUserHandler : IRequestHandler<GetExpenseByUserQuery, IReadOnlyList<ExpenseDto>>
{
    private readonly IExpenseRepository _expenseRepository;

    public GetExpenseByUserHandler(IExpenseRepository expenseRepository)
    {
        _expenseRepository = expenseRepository;
    }

    public async Task<IReadOnlyList<ExpenseDto>> Handle(GetExpenseByUserQuery query, CancellationToken ct)
    {
        IReadOnlyList<Expense> expenses = await _expenseRepository.GetByUserIdAsync(query.UserId, ct);

        IReadOnlyList<ExpenseDto> expenseDtos = expenses.Select(e =>
            new ExpenseDto(
                e.Id,
                query.UserId,
                e.Amount,
                e.Category,
                e.ExpenseDate,
                e.Description
            )).ToList();

        return expenseDtos;
    }
}
