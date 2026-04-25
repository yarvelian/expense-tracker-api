using ExpenseTracker.Application.Abstractions;
using ExpenseTracker.Domain.Entities;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Update;

public sealed class UpdateExpenseHandler : IRequestHandler<UpdateExpenseCommand, Unit>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateExpenseHandler(
        IExpenseRepository expenseRepository,
        IUnitOfWork unitOfWork)
    {
        _expenseRepository  = expenseRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Unit> Handle(UpdateExpenseCommand command, CancellationToken ct)
    {
        Expense? expense = await _expenseRepository.GetByIdAsync(command.ExpenseId, ct);
        if (expense == null)
        {
            throw new InvalidOperationException("Expense not found");
        }

        if (expense.UserId != command.UserId)
        {
            throw new InvalidOperationException("Expense not found");
        }
        
        expense.Update(command.Amount, command.ExpenseCategory, command.ExpenseDate, command.Description);

        _expenseRepository.Update(expense);
        await _unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
