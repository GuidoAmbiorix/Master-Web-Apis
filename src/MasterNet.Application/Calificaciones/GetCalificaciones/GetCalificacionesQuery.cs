using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MasterNet.Application.Core;
using MasterNet.Domain;
using MasterNet.Persistence;
using MediatR;

namespace MasterNet.Application.Calificaciones.GetCalificaciones;


public record GetCalificacionesQueryRequest:IRequest<Result<PagedList<CalificacionResponse>>>
{
    public GetCalificacionesRequest? CalificacionesRequest { get; set; }
}

internal class GetCalificacionesQueryHandler : IRequestHandler<GetCalificacionesQueryRequest, Result<PagedList<CalificacionResponse>>>
{
     private readonly MasterNetDbContext _context;
        private readonly IMapper _mapper;

        public GetCalificacionesQueryHandler(MasterNetDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        
    public async Task<Result<PagedList<CalificacionResponse>>> Handle(GetCalificacionesQueryRequest request, CancellationToken cancellationToken)
    {
        IQueryable<Calificacion> queryable = _context.Calificaciones!;

           var predicate = ExpressionBuilder.New<Calificacion>();

           if(!string.IsNullOrEmpty(request.CalificacionesRequest!.Alumno))
           {
            predicate = predicate.And(y => y.Alumno!.Contains(request.CalificacionesRequest!.Alumno));
           }

           if(request.CalificacionesRequest!.CursoId is not null)
           {
            predicate = predicate.And(y => y.CursoId! == request.CalificacionesRequest!.CursoId);
           }

           if(!string.IsNullOrEmpty(request.CalificacionesRequest.OrderBy))
           {
            Expression<Func<Calificacion,object>>? orderBySelector =
            request.CalificacionesRequest.OrderBy.ToLower() switch
            {
                "nombre" => alumno => alumno.Alumno!,
                "curso" => alumno => alumno.CursoId!,
                _ => alumno => alumno.Alumno!
            };

            bool orderBy = request.CalificacionesRequest.OrderAsc.HasValue
            ? request.CalificacionesRequest.OrderAsc.Value
            : true;

            queryable = orderBy ? queryable.OrderBy(orderBySelector) : queryable.OrderByDescending(orderBySelector);

           }

           queryable = queryable.Where(predicate);

           var instructoresQuery =
            queryable
            .ProjectTo<CalificacionResponse>(_mapper.ConfigurationProvider)
            .AsQueryable();

            var pagination = await PagedList<CalificacionResponse>.CreateAsync(instructoresQuery,request.CalificacionesRequest.PageNumber,request.CalificacionesRequest.PageSize);

            return Result<PagedList<CalificacionResponse>>.Success(pagination);
    }
}

public record CalificacionResponse(
    string? Alumno,
    int? Puntaje,
    string? Comentario,
    string? NombreCurso
)
{
    public CalificacionResponse(): this(null, null, null, null)
    {
    }
}