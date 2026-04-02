using ExpenseTracker.Api.Models;
using ExpenseTracker.Application.Features.User.Login;
using ExpenseTracker.Application.Features.User.Registration;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Api.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        var command = new RegisterUserCommand(request.Email, request.Password);
        var result = await _mediator.Send(command);
        
        return StatusCode(StatusCodes.Status201Created, new { id = result });
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
    {
        var command = new LoginCommand(request.Email, request.Password);
        var result = await _mediator.Send(command);
        
        return Ok(new { token = result });
    }
}
