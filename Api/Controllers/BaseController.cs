using Api.Helpers;
using Application.Handlers.ClientHandlers;
using Application.Handlers.CommonHandlers;
using Domain.Entities ;
using InvoiceHub.Application.Handlers.CommonHandlers;
using InvoiceHub.Application.Requests.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Queries.QueryBuilderEngine;
using Domain.Entites;
namespace InvoiceHub.Api.Controllers;

public class BaseController<TEntity> : ControllerBase
    where TEntity : BaseDomainEntity

{
    protected IMediator mediator =>
        HttpContext.RequestServices.GetRequiredService<IMediator>();

    protected QueryOptions options =>
        HttpContext.RequestServices.GetRequiredService<QueryOptions>();
 
    [HttpGet]
    public async Task<ActionResult<PaginatedResult<TEntity>>> GetAllClients(CancellationToken ct)
    {
        var result = await mediator.Send(new GetAll<TEntity>(), ct);
        return Ok(new PaginatedResult<TEntity>(result, options));
    }

    [HttpGet("{Id:Guid}")]
    public async Task<ActionResult<ResponseDto<TEntity>?>> GetById(GetTEntityByIdRequest<TEntity> request, CancellationToken ct)
    {
        var result = await mediator.Send(request, ct);
        if (result is null)
            return NotFound();
        return Ok(result);
    }

    [HttpPost("create")]
    public async Task<ActionResult<TEntity>> CreateClient(CreateRequest<TEntity> request, CancellationToken ct)
    {
        var result = await mediator.Send(request, ct);
        return Ok(result);
    }

}