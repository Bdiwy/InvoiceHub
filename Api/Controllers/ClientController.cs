using Domain.Entities ;
using Microsoft.AspNetCore.Mvc;
namespace InvoiceHub.Api.Controllers;

[ApiController]
[Route("api/clients")]
public class ClientController  : BaseController<Client>{}