using ExpenseTracker.Application.Behaviors;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using NSubstitute;

namespace ExpenseTracker.Application.Tests.Behaviors;

public sealed record TestRequest(string Value) : IRequest<string>;

public sealed class ValidationBehaviorTests
{

    [Fact]
    public async Task Handle_WhenNoValidators_ShouldCallNext()
    {
        var behavior = new ValidationBehavior<TestRequest, string>([]);
        var next = Substitute.For<RequestHandlerDelegate<string>>();
        next.Invoke().Returns("result");

        var request = new TestRequest("test");
        
        var result = await behavior.Handle(request, next, CancellationToken.None);
        
        result.Should().Be("result");
        await next.Received(1).Invoke();
    }

    [Fact]
    public async Task Handle_WhenValidationPasses_ShouldCallNext()
    {
        var validator = Substitute.For<IValidator<TestRequest>>();
        validator.Validate(Arg.Any<TestRequest>())
            .Returns(new ValidationResult());

        var behavior = new ValidationBehavior<TestRequest, string>([validator]);
        var next = Substitute.For<RequestHandlerDelegate<string>>();
        next.Invoke().Returns("result");

        var request = new TestRequest("valid");
        
        var result = await behavior.Handle(request, next, CancellationToken.None);
        
        result.Should().Be("result");
        await next.Received(1).Invoke();
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldThrowValidationException()
    {
        // Arrange
        var failures = new List<ValidationFailure>
        {
            new("Value", "Value is required"),
            new("Value", "Value is too short")
        };

        var validator = Substitute.For<IValidator<TestRequest>>();
        validator.Validate(Arg.Any<TestRequest>())
            .Returns(new ValidationResult(failures));

        var behavior = new ValidationBehavior<TestRequest, string>([validator]);
        var next = Substitute.For<RequestHandlerDelegate<string>>();

        var request = new TestRequest("");

        // Act
        var act = async () => await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .Where(ex => ex.Errors.Count() == 2);
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldNotCallNext()
    {
        // Arrange
        var failures = new List<ValidationFailure>
        {
            new("Value", "Value is required")
        };

        var validator = Substitute.For<IValidator<TestRequest>>();
        validator.Validate(Arg.Any<TestRequest>())
            .Returns(new ValidationResult(failures));

        var behavior = new ValidationBehavior<TestRequest, string>([validator]);
        var next = Substitute.For<RequestHandlerDelegate<string>>();

        var request = new TestRequest("");

        // Act
        var act = async () => await behavior.Handle(request, next, CancellationToken.None);
        await act.Should().ThrowAsync<ValidationException>();

        // Assert
        await next.DidNotReceive().Invoke();
    }

    [Fact]
    public async Task Handle_WhenMultipleValidators_ShouldCollectAllFailures()
    {
        // Arrange
        var validator1 = Substitute.For<IValidator<TestRequest>>();
        validator1.Validate(Arg.Any<TestRequest>())
            .Returns(new ValidationResult([new ValidationFailure("Value", "Error from validator 1")]));

        var validator2 = Substitute.For<IValidator<TestRequest>>();
        validator2.Validate(Arg.Any<TestRequest>())
            .Returns(new ValidationResult([new ValidationFailure("Value", "Error from validator 2")]));

        var behavior = new ValidationBehavior<TestRequest, string>([validator1, validator2]);
        var next = Substitute.For<RequestHandlerDelegate<string>>();

        var request = new TestRequest("");

        // Act
        var act = async () => await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .Where(ex => ex.Errors.Count() == 2);
    }
}
