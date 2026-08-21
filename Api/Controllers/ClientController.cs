using Api.Helpers;
using Application.Handlers.ClientHandlers;
using Domain.Entities ;
using InvoiceHub.Application.Handlers.ClientHandlers;
using InvoiceHub.Application.Requests.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Queries.QueryBuilderEngine;
namespace InvoiceHub.Api.Controllers;

[ApiController]
[Route("api/clients")]
public class ClientController(IMediator mediator, QueryOptions options)  : ControllerBase
{

    [HttpGet]
    public async Task<ActionResult<PaginatedResult<ClientsResponseDto>>> GetAllClients(CancellationToken ct)
    {
        var result = await mediator.Send(new GetAllClientsRequest(), ct);
        return Ok(new PaginatedResult<ClientsResponseDto>(result, options)); 
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