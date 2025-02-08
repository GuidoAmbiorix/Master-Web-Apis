using MasterNet.Application.Precios.GetPrecios;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MasterNet.WebApi.Controllers;

[ApiController]
[Route("api/precios")]
public class PreciosController : ControllerBase
{
    private readonly ISender _sender;
    public PreciosController(ISender sender)   
    {
        _sender = sender;
    }


    [HttpGet]
    public async Task<IActionResult> PaginationPrecios([FromQuery] GetPreciosRequest request, CancellationToken cancellationToken)
    {
       var query = new GetPreciosQueryRequest { PreciosRequest = request};
       var resultado = await _sender.Send(query,cancellationToken);
       return resultado.IsSuccess ? Ok(resultado.Value) : NotFound();
    }

}