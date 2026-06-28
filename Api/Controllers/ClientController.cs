using Application.Handlers.ClientHandlers;
using Domain.Entities ;
using InvoiceHub.Application.Handlers.ClientHandlers;
using InvoiceHub.Application.Requests.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace InvoiceHub.Api.Controllers;

[ApiController]
[Route("api/clients")]
public class ClientController(IMediator mediator)  : ControllerBase
{

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClientsResponseDto>>> GetAllClients(CancellationToken ct)
    {
        var result = await mediator.Send(new GetAllClientsRequest(), ct);
        return Ok(result); 
    }

    [HttpGet("{clientId:Guid}")]
    public async Task<ActionResult<ClientResponseDto?>> GetClientById(GetClientByIdRequest request , CancellationToken ct)
    {
        var result = await mediator.Send(request, ct);
        if(result is null)
            return NotFound();
        return Ok(result); 
    }

    [HttpPost("create")]
    public async Task<ActionResult<ClientResponseDto>> CreateClient(CreateClientRequest request , CancellationToken ct)
    {
        var result = await mediator.Send(request, ct);
        return Ok(result); 
    }

    
}