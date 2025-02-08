using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MasterNet.Application.Core;
using MasterNet.Domain;
using MasterNet.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace MasterNet.Application.Precios.GetPrecios;

public record GetPreciosQueryRequest : IRequest<Result<PagedList<PrecioResponse>>>
{
    public GetPreciosRequest? PreciosRequest { get; set; }
}

internal class GetPreciosQueryHandler : IRequestHandler<GetPreciosQueryRequest, Result<PagedList<PrecioResponse>>>
{
    
        private readonly MasterNetDbContext _context;
        private readonly IMapper _mapper;

        public GetPreciosQueryHandler(MasterNetDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        
    public async Task<Result<PagedList<PrecioResponse>>> Handle(GetPreciosQueryRequest request, CancellationToken cancellationToken)
    {
        IQueryable<Precio> queryable = _context.Precios!;

           var predicate = ExpressionBuilder.New<Precio>();

           if(!string.IsNullOrEmpty(request.PreciosRequest!.Nombre))
           {
            predicate = predicate.And(y => y.Nombre!.Contains(request.PreciosRequest!.Nombre));
           }

           if(!string.IsNullOrEmpty(request.PreciosRequest.OrderBy))
           {
            Expression<Func<Precio,object>>? orderBySelector =
            request.PreciosRequest.OrderBy.ToLower() switch
            {
                "nombre" => precio => precio.Nombre!,
                "precio" => precio => precio.PrecioActual!,
                _ => precio => precio.Nombre!
            };

            bool orderBy = request.PreciosRequest.OrderAsc.HasValue
            ? request.PreciosRequest.OrderAsc.Value
            : true;

            queryable = orderBy ? queryable.OrderBy(orderBySelector) : queryable.OrderByDescending(orderBySelector);

           }

           queryable = queryable.Where(predicate);

           var preciosQuery =
            queryable
            .ProjectTo<PrecioResponse>(_mapper.ConfigurationProvider)
            .AsQueryable();

            var pagination = await PagedList<PrecioResponse>.CreateAsync(preciosQuery,request.PreciosRequest.PageNumber,request.PreciosRequest.PageSize);

            return Result<PagedList<PrecioResponse>>.Success(pagination);
    }
}

public record PrecioResponse(
    Guid? Id,
    string? Nombre,
    decimal? PrecioActual,
    decimal? PrecioPromocion
)
{
    public PrecioResponse(): this(null, null, null, null)
    {
    }
}