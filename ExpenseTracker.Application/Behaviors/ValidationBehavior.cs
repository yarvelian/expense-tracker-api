using FluentValidation;
using MediatR;

namespace ExpenseTracker.Application.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) :
    IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var failures = validators.Select(v => v.Validate(request))
            .SelectMany(result => result.Errors)
            .ToList();
        
        if (failures.Count > 0)
        {
            throw new ValidationException(failures);
        }
        
        return await next(cancellationToken);
    }

}
