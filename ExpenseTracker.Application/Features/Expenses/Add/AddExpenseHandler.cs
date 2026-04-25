using ExpenseTracker.Application.Abstractions;
using ExpenseTracker.Domain.Entities;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Add;

public sealed class AddExpenseHandler : IRequestHandler<AddExpenseCommand, Guid>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddExpenseHandler(
        IExpenseRepository expenseRepository,
        IUnitOfWork unitOfWork)
    {
        _expenseRepository  = expenseRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Guid> Handle(AddExpenseCommand command, CancellationToken ct)
    {
        Expense expense = new Expense(
            command.UserId, 
            command.Amount, 
            command.ExpenseCategory, 
            command.ExpenseDate, 
            command.Description
        );

        await _expenseRepository.AddAsync(expense, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return expense.Id;
    }
}
