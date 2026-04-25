using ExpenseTracker.Application.Abstractions;
using ExpenseTracker.Application.Features.Expenses.Shared;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Exceptions;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.GetById;

public sealed class GetExpenseByIdHandler : IRequestHandler<GetExpenseByIdQuery, ExpenseDto>
{
    private readonly IExpenseRepository _expenseRepository;

    public GetExpenseByIdHandler(IExpenseRepository expenseRepository)
    {
        _expenseRepository  = expenseRepository;
    }
    
    public async Task<ExpenseDto> Handle(GetExpenseByIdQuery query, CancellationToken ct)
    {
        Expense? expense = await _expenseRepository.GetByIdAsync(query.ExpenseId, ct);
        if (expense == null || expense.UserId != query.UserId)
        {
            throw new NotFoundException("Expense not found");
        }
        
        ExpenseDto expenseDto = new ExpenseDto(
            expense.Id,
            query.UserId,
            expense.Amount,
            expense.Category,
            expense.ExpenseDate,
            expense.Description);

        return expenseDto;
    }
}
