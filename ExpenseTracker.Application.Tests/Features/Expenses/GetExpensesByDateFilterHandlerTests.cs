using ExpenseTracker.Application.Abstractions;
using ExpenseTracker.Application.Features.Expenses.GetByDateFilter;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using FluentAssertions;
using NSubstitute;

namespace ExpenseTracker.Application.Tests.Features.Expenses;

public sealed class GetExpensesByDateFilterHandlerTests
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly GetExpensesByDateFilterHandler _handler;

    public GetExpensesByDateFilterHandlerTests()
    {
        _expenseRepository = Substitute.For<IExpenseRepository>();
        _handler = new GetExpensesByDateFilterHandler(_expenseRepository);
    }

    [Fact]
    public async Task Handle_WhenExpensesExist_ShouldReturnMappedDtos()
    {
        var userId = Guid.NewGuid();
        var expenses = new List<Expense>
        {
            new(userId, 100m, ExpenseCategory.Groceries, DateTime.UtcNow.AddDays(-3), null),
            new(userId, 200m, ExpenseCategory.Leisure, DateTime.UtcNow.AddDays(-5), null)
        };
        var query = new GetExpensesByDateFilterQuery(userId, ExpenseDateFilter.PastWeek, null, null);

        _expenseRepository.GetFilteredAsync(userId, ExpenseDateFilter.PastWeek, null, null, Arg.Any<CancellationToken>())
            .Returns(expenses);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_WhenNoExpenses_ShouldReturnEmptyList()
    {
        var userId = Guid.NewGuid();
        var query = new GetExpensesByDateFilterQuery(userId, ExpenseDateFilter.PastMonth, null, null);

        _expenseRepository.GetFilteredAsync(userId, ExpenseDateFilter.PastMonth, null, null, Arg.Any<CancellationToken>())
            .Returns(new List<Expense>());

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WhenCustomFilter_ShouldPassDatesToRepository()
    {
        var userId = Guid.NewGuid();
        var startDate = DateTime.UtcNow.AddDays(-10);
        var endDate = DateTime.UtcNow;
        var query = new GetExpensesByDateFilterQuery(userId, ExpenseDateFilter.Custom, startDate, endDate);

        _expenseRepository.GetFilteredAsync(userId, ExpenseDateFilter.Custom, startDate, endDate, Arg.Any<CancellationToken>())
            .Returns(new List<Expense>());

        await _handler.Handle(query, CancellationToken.None);

        await _expenseRepository.Received(1)
            .GetFilteredAsync(userId, ExpenseDateFilter.Custom, startDate, endDate, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenExpensesExist_ShouldMapAllFieldsCorrectly()
    {
        var userId = Guid.NewGuid();
        var expenseDate = DateTime.UtcNow.AddDays(-2);
        var expense = new Expense(userId, 150m, ExpenseCategory.Health, expenseDate, "Medical");
        var query = new GetExpensesByDateFilterQuery(userId, ExpenseDateFilter.PastWeek, null, null);

        _expenseRepository.GetFilteredAsync(userId, ExpenseDateFilter.PastWeek, null, null, Arg.Any<CancellationToken>())
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
