using System.Security.Claims;
using ExpenseTracker.Api.Models;
using ExpenseTracker.Application.Features.Expenses.Add;
using ExpenseTracker.Application.Features.Expenses.Delete;
using ExpenseTracker.Application.Features.Expenses.GetById;
using ExpenseTracker.Application.Features.Expenses.GetByUser;
using ExpenseTracker.Application.Features.Expenses.Shared;
using ExpenseTracker.Application.Features.Expenses.Update;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/expenses")]
public sealed class ExpensesController : ControllerBase
{
    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    
    private readonly IMediator _mediator;
    
    public ExpensesController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost]
    public async Task<ActionResult<Guid>> AddExpense([FromBody] AddExpenseRequest request)
    {
        var command = new AddExpenseCommand(
            CurrentUserId, 
            request.Amount, 
            request.ExpenseCategory, 
            request.ExpenseDate, 
            request.Description
        );
        
        var result = await _mediator.Send(command);
        
        return StatusCode(StatusCodes.Status201Created, new { id = result });
    }
    
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateExpense([FromBody] UpdateExpenseRequest request, [FromRoute] Guid id)
    {
        var command = new UpdateExpenseCommand(
            CurrentUserId,
            id, 
            request.Amount, 
            request.ExpenseCategory, 
            request.ExpenseDate, 
            request.Description
        );
        
        await _mediator.Send(command);
        
        return StatusCode(StatusCodes.Status204NoContent);
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteExpense([FromRoute] Guid id)
    {
        var command = new DeleteExpenseCommand(CurrentUserId, id);
        
        await _mediator.Send(command);
        
        return StatusCode(StatusCodes.Status204NoContent);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ExpenseDto>> GetExpenseById([FromRoute] Guid id)
    {
        var query = new GetExpenseByIdQuery(CurrentUserId, id);
        
        var result = await _mediator.Send(query);
        
        return Ok(result);
    }
    
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ExpenseDto>>> GetExpenseByUserId()
    {
        var query = new GetExpenseByUserQuery(CurrentUserId);
        
        var result = await _mediator.Send(query);
        
        return Ok(result);
    }
}
