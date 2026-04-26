using ExpenseTracker.Application.Abstractions;
using ExpenseTracker.Application.Features.Expenses.Delete;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.Exceptions;
using FluentAssertions;
using MediatR;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace ExpenseTracker.Application.Tests.Features.Expenses;

public sealed class DeleteExpenseHandlerTests
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly DeleteExpenseHandler _handler;

    public DeleteExpenseHandlerTests()
    {
        _expenseRepository = Substitute.For<IExpenseRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new DeleteExpenseHandler(_expenseRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_WhenExpenseExists_ShouldRemoveAndSave()
    {
        var userId = Guid.NewGuid();
        var expense = new Expense(userId, 100m, ExpenseCategory.Groceries, DateTime.UtcNow, null);
        var command = new DeleteExpenseCommand(userId, expense.Id);

        _expenseRepository.GetByIdAsync(command.ExpenseId, Arg.Any<CancellationToken>())
            .Returns(expense);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(Unit.Value);
        _expenseRepository.Received(1).Remove(expense);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenExpenseNotFound_ShouldThrowNotFoundException()
    {
        var command = new DeleteExpenseCommand(Guid.NewGuid(), Guid.NewGuid());

        _expenseRepository.GetByIdAsync(command.ExpenseId, Arg.Any<CancellationToken>())
            .ReturnsNull();

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Expense not found");
    }

    [Fact]
    public async Task Handle_WhenExpenseBelongsToAnotherUser_ShouldThrowNotFoundException()
    {
        var expense = new Expense(Guid.NewGuid(), 100m, ExpenseCategory.Groceries, DateTime.UtcNow, null);
        var command = new DeleteExpenseCommand(Guid.NewGuid(), expense.Id);

        _expenseRepository.GetByIdAsync(command.ExpenseId, Arg.Any<CancellationToken>())
            .Returns(expense);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Expense not found");

        _expenseRepository.DidNotReceive().Remove(Arg.Any<Expense>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenExpenseNotFound_ShouldNotSaveChanges()
    {
        var command = new DeleteExpenseCommand(Guid.NewGuid(), Guid.NewGuid());

        _expenseRepository.GetByIdAsync(command.ExpenseId, Arg.Any<CancellationToken>())
            .ReturnsNull();

        var act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
