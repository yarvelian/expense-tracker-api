using System.Reflection;
using ExpenseTracker.Application.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;


namespace ExpenseTracker.Application;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplication()
        {
            services.AddMediatR(cfg => 
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            
            return services;
        }
    }
}