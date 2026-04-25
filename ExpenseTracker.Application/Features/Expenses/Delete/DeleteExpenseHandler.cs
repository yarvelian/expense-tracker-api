using ExpenseTracker.Application.Abstractions;
using ExpenseTracker.Domain.Entities;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Delete;

public sealed class DeleteExpenseHandler : IRequestHandler<DeleteExpenseCommand, Unit>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteExpenseHandler(
        IExpenseRepository expenseRepository,
        IUnitOfWork unitOfWork)
    {
        _expenseRepository  = expenseRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Unit> Handle(DeleteExpenseCommand command, CancellationToken ct)
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
        
        _expenseRepository.Remove(expense);
        await _unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
