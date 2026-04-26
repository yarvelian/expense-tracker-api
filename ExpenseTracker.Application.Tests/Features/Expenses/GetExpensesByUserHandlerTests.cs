using ExpenseTracker.Application.Abstractions;
using ExpenseTracker.Application.Features.Expenses.GetByUser;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using FluentAssertions;
using NSubstitute;

namespace ExpenseTracker.Application.Tests.Features.Expenses;

public sealed class GetExpensesByUserHandlerTests
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly GetExpenseByUserHandler _handler;

    public GetExpensesByUserHandlerTests()
    {
        _expenseRepository = Substitute.For<IExpenseRepository>();
        _handler = new GetExpenseByUserHandler(_expenseRepository);
    }

    [Fact]
    public async Task Handle_WhenExpensesExist_ShouldReturnMappedDtos()
    {
        var userId = Guid.NewGuid();
        var expenses = new List<Expense>
        {
            new(userId, 100m, ExpenseCategory.Groceries, DateTime.UtcNow, "First"),
            new(userId, 200m, ExpenseCategory.Leisure, DateTime.UtcNow.AddDays(-1), "Second")
        };
        var query = new GetExpenseByUserQuery(userId);

        _expenseRepository.GetByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(expenses);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().HaveCount(2);
        result[0].Amount.Should().Be(100m);
        result[1].Amount.Should().Be(200m);
    }

    [Fact]
    public async Task Handle_WhenNoExpenses_ShouldReturnEmptyList()
    {
        var userId = Guid.NewGuid();
        var query = new GetExpenseByUserQuery(userId);

        _expenseRepository.GetByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(new List<Expense>());

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WhenExpensesExist_ShouldMapAllFieldsCorrectly()
    {
        var userId = Guid.NewGuid();
        var expenseDate = DateTime.UtcNow;
        var expense = new Expense(userId, 150m, ExpenseCategory.Health, expenseDate, "Medical");
        var query = new GetExpenseByUserQuery(userId);

        _expenseRepository.GetByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(new List<Expense> { expense });

        var result = await _handler.Handle(query, CancellationToken.None);

        var dto = result[0];
        dto.Id.Should().Be(expense.Id);
        dto.UserId.Should().Be(userId);
        dto.Amount.Should().Be(150m);
        dto.ExpenseCategory.Should().Be(ExpenseCategory.Health);
        dto.ExpenseDate.Should().Be(expenseDate);
        dto.Description.Should().Be("Medical");
    }
}
