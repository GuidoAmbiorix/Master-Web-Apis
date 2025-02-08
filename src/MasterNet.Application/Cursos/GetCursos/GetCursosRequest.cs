namespace  MasterNet.Application.Core.GetCursos;

public class GetCursosRequest : PagingParams
{
    public string? Titulo {get;set;}
    public string? Descripcion { get; set; }
}
