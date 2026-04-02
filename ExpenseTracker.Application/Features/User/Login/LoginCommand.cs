using MediatR;

namespace ExpenseTracker.Application.Features.User.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<string>;
