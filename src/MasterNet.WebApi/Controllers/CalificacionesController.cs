
using MasterNet.Application.Calificaciones.GetCalificaciones;
using MasterNet.Application.Precios.GetPrecios;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MasterNet.WebApi.Controllers;

[ApiController]
[Route("api/calificaciones")]
public class CalificacionesController : ControllerBase
{
    private readonly ISender _sender;
    public CalificacionesController(ISender sender)   
    {
        _sender = sender;
    }


    [HttpGet]
    public async Task<IActionResult> PaginationCalificaiones([FromQuery] GetCalificacionesRequest request, CancellationToken cancellationToken)
    {
       var query = new GetCalificacionesQueryRequest { CalificacionesRequest = request};
       var resultado = await _sender.Send(query,cancellationToken);
       return resultado.IsSuccess ? Ok(resultado.Value) : NotFound();
    }

}