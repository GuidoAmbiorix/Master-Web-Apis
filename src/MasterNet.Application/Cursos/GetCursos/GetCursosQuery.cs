using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MasterNet.Application.Core;
using MasterNet.Application.Core.GetCursos;
using MasterNet.Application.Cursos.GetCurso;
using MasterNet.Domain;
using MasterNet.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MasterNet.Application.Cursos.GetCursos;


public class GetCursosQuery
{
    public record GetCursosQueryRequest: IRequest<Result<PagedList<CursoResponse>>>
    {
        public GetCursosRequest? CursosRequest {get;set;}
    }

    internal class GetCursosQueryHandler : IRequestHandler<GetCursosQueryRequest, Result<PagedList<CursoResponse>>>
    {
        private readonly MasterNetDbContext _context;
        private readonly IMapper _mapper;

        public GetCursosQueryHandler(MasterNetDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<PagedList<CursoResponse>>> Handle(GetCursosQueryRequest request, CancellationToken cancellationToken)
        {
           IQueryable<Curso> queryable = _context.Cursos!
           .Include(x => x.Instructores)
           .Include(x => x.Calificaciones)
           .Include(x => x.Precios);

            var predicate = ExpressionBuilder.New<Curso>();
            if(!string.IsNullOrEmpty(request.CursosRequest!.Titulo))
              {
                predicate = 
                predicate
                .And(y => y.Titulo!.ToLower()
                .Contains(request.CursosRequest.Titulo.ToLower()));
              }

             if(!string.IsNullOrEmpty(request.CursosRequest!.Descripcion))
              {
                predicate = 
                predicate
                .And(y => y.Descripcion!.ToLower()
                .Contains(request.CursosRequest.Descripcion.ToLower()));
              }
            
             if(!string.IsNullOrEmpty(request.CursosRequest!.OrderBy))
              {
                Expression<Func<Curso,object>> OrderBySelector = request.CursosRequest.OrderBy!.ToLower() switch
                {
                   "titulo" => Curso => Curso.Titulo!,
                   "descripcion" => Curso => Curso.Descripcion!,
                   _ => Curso => Curso.Titulo!,
                };

                bool orderBy = request.CursosRequest.OrderAsc.HasValue 
                ? request.CursosRequest.OrderAsc.Value
                : true; 

                queryable = orderBy 
                            ? queryable.OrderBy(OrderBySelector)
                            : queryable.OrderByDescending(OrderBySelector);
              }


              queryable = queryable.Where(predicate);

              var cursosQuery = queryable.ProjectTo<CursoResponse>(_mapper.ConfigurationProvider).AsQueryable();

             var pagination =  await PagedList<CursoResponse>.CreateAsync(cursosQuery,request.CursosRequest.PageNumber,request.CursosRequest.PageSize);  

             return Result<PagedList<CursoResponse>>.Success(pagination);

        }
    }
}