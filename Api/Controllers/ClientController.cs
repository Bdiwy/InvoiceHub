using InvoiceHub.Application.Requests.DTOs;
using InvoiceHub.Application.Handlers.ClientHandlers;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Domain.Entities ;
namespace InvoiceHub.Api.Controllers;

[ApiController]
[Route("api/clients")]
public class ClientController(IMediator mediator)  : ControllerBase
{
    [HttpGet("get-client-by-id/{clientId:Guid}")]
    public async Task<ActionResult<ClientResponseDto?>> GetClientById([FromRoute] Guid clientId , CancellationToken ct)
    {
        var result = await mediator.Send(new GetClientByIdHandler(clientId), ct);
        if(result is null)
            return NotFound();
        return Ok(result); 
    }

    [HttpGet("get-all-clients")]
    public async Task<ActionResult<IEnumerable<ClientsResponseDto>>> GetAll(CancellationToken ct)
    {
        var result = await mediator.Send(new GetAllClientsHandler(), ct);
        return Ok(result); 
    }

    [HttpPost("create-client")]
    public async Task<ActionResult<ClientResponseDto>> CreateClient([FromBody] CreateClientRequestDto request , CancellationToken ct)
    {
        var result = await mediator.Send(new CreateClientHandler(request), ct);
        return Ok(result); 
    }

    
}