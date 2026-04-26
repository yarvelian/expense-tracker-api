using ExpenseTracker.Application.Abstractions;
using ExpenseTracker.Application.Features.Expenses.GetById;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.Exceptions;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace ExpenseTracker.Application.Tests.Features.Expenses;

public sealed class GetExpenseByIdHandlerTests
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly GetExpenseByIdHandler _handler;

    public GetExpenseByIdHandlerTests()
    {
        _expenseRepository = Substitute.For<IExpenseRepository>();
        _handler = new GetExpenseByIdHandler(_expenseRepository);
    }

    [Fact]
    public async Task Handle_WhenExpenseExists_ShouldReturnExpenseDto()
    {
        var userId = Guid.NewGuid();
        var expense = new Expense(userId, 100m, ExpenseCategory.Groceries, DateTime.UtcNow, "Test");
        var query = new GetExpenseByIdQuery(userId, expense.Id);

        _expenseRepository.GetByIdAsync(query.ExpenseId, Arg.Any<CancellationToken>())
            .Returns(expense);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Id.Should().Be(expense.Id);
        result.UserId.Should().Be(userId);
        result.Amount.Should().Be(expense.Amount);
        result.ExpenseCategory.Should().Be(expense.Category);
        result.ExpenseDate.Should().Be(expense.ExpenseDate);
        result.Description.Should().Be(expense.Description);
    }

    [Fact]
    public async Task Handle_WhenExpenseNotFound_ShouldThrowNotFoundException()
    {
        var query = new GetExpenseByIdQuery(Guid.NewGuid(), Guid.NewGuid());

        _expenseRepository.GetByIdAsync(query.ExpenseId, Arg.Any<CancellationToken>())
            .ReturnsNull();

        var act = async () => await _handler.Handle(query, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Expense not found");
    }

    [Fact]
    public async Task Handle_WhenExpenseBelongsToAnotherUser_ShouldThrowNotFoundException()
    {
        var expense = new Expense(Guid.NewGuid(), 100m, ExpenseCategory.Groceries, DateTime.UtcNow, null);
        var query = new GetExpenseByIdQuery(Guid.NewGuid(), expense.Id);

        _expenseRepository.GetByIdAsync(query.ExpenseId, Arg.Any<CancellationToken>())
            .Returns(expense);

        var act = async () => await _handler.Handle(query, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Expense not found");
    }
}
