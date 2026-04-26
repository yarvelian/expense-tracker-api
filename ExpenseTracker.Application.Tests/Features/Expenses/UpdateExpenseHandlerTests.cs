using ExpenseTracker.Application.Abstractions;
using ExpenseTracker.Application.Features.Expenses.Update;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.Exceptions;
using FluentAssertions;
using MediatR;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace ExpenseTracker.Application.Tests.Features.Expenses;

public sealed class UpdateExpenseHandlerTests
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UpdateExpenseHandler _handler;

    public UpdateExpenseHandlerTests()
    {
        _expenseRepository = Substitute.For<IExpenseRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new UpdateExpenseHandler(_expenseRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_WhenExpenseExists_ShouldUpdateAndSave()
    {
        var userId = Guid.NewGuid();
        var expense = new Expense(userId, 50m, ExpenseCategory.Health, DateTime.UtcNow.AddDays(-1), null);
        var command = new UpdateExpenseCommand(
            userId,
            expense.Id,
            200m,
            ExpenseCategory.Leisure,
            DateTime.UtcNow,
            "Updated");

        _expenseRepository.GetByIdAsync(command.ExpenseId, Arg.Any<CancellationToken>())
            .Returns(expense);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(Unit.Value);
        expense.Amount.Should().Be(command.Amount);
        expense.Category.Should().Be(command.ExpenseCategory);
        expense.ExpenseDate.Should().Be(command.ExpenseDate);
        expense.Description.Should().Be(command.Description);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenExpenseNotFound_ShouldThrowNotFoundException()
    {
        var command = new UpdateExpenseCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            100m,
            ExpenseCategory.Groceries,
            DateTime.UtcNow,
            null);

        _expenseRepository.GetByIdAsync(command.ExpenseId, Arg.Any<CancellationToken>())
            .ReturnsNull();

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Expense not found");
    }

    [Fact]
    public async Task Handle_WhenExpenseBelongsToAnotherUser_ShouldThrowNotFoundException()
    {
        var expense = new Expense(Guid.NewGuid(), 50m, ExpenseCategory.Health, DateTime.UtcNow, null);
        var command = new UpdateExpenseCommand(
            Guid.NewGuid(),
            expense.Id,
            100m,
            ExpenseCategory.Groceries,
            DateTime.UtcNow,
            null);

        _expenseRepository.GetByIdAsync(command.ExpenseId, Arg.Any<CancellationToken>())
            .Returns(expense);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Expense not found");

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenExpenseNotFound_ShouldNotSaveChanges()
    {
        var command = new UpdateExpenseCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            100m,
            ExpenseCategory.Groceries,
            DateTime.UtcNow,
            null);

        _expenseRepository.GetByIdAsync(command.ExpenseId, Arg.Any<CancellationToken>())
            .ReturnsNull();

        var act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
