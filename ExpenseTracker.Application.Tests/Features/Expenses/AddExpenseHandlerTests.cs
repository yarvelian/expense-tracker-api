using ExpenseTracker.Application.Abstractions;
using ExpenseTracker.Application.Features.Expenses.Add;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using FluentAssertions;
using NSubstitute;

namespace ExpenseTracker.Application.Tests.Features.Expenses;

public sealed class AddExpenseHandlerTests
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly AddExpenseHandler _handler;

    public AddExpenseHandlerTests()
    {
        _expenseRepository = Substitute.For<IExpenseRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new AddExpenseHandler(_expenseRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_WhenCommandIsValid_ShouldReturnNewExpenseId()
    {
        var command = new AddExpenseCommand(
            Guid.NewGuid(),
            100m,
            ExpenseCategory.Groceries,
            DateTime.UtcNow,
            "Test expense");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_WhenCommandIsValid_ShouldAddExpenseToRepository()
    {
        var command = new AddExpenseCommand(
            Guid.NewGuid(),
            100m,
            ExpenseCategory.Groceries,
            DateTime.UtcNow,
            "Test expense");

        await _handler.Handle(command, CancellationToken.None);

        await _expenseRepository.Received(1)
            .AddAsync(Arg.Is<Expense>(e =>
                    e.UserId == command.UserId &&
                    e.Amount == command.Amount &&
                    e.Category == command.ExpenseCategory),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCommandIsValid_ShouldSaveChanges()
    {
        var command = new AddExpenseCommand(
            Guid.NewGuid(),
            100m,
            ExpenseCategory.Groceries,
            DateTime.UtcNow,
            null);

        await _handler.Handle(command, CancellationToken.None);

        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
