using BuildingBlocks.Infrastructure.DataAccess.Cosmos.Extensions;
using InteriorDesigners.Application.Products.GetProducts;
using InteriorDesigners.Domain.InteriorDesigner;
using InteriorDesigners.Infrastructure.DataAccess;
using MediatR;

namespace InteriorDesigners.Infrastructure.Queries.InteriorDesigners;

public class GetInteriorDesignerTourInfoPresetsQueryHandler : IRequestHandler<GetInteriorDesignerTourInfoPresetsQuery,GetInteriorDesignerTourInfoPresetsQueryResult>
{
    private readonly InteriorDesignerContext _interiorDesignerContext;


    public GetInteriorDesignerTourInfoPresetsQueryHandler(InteriorDesignerContext interiorDesignerContext)
    {
        _interiorDesignerContext = interiorDesignerContext;
    }

    public async Task<GetInteriorDesignerTourInfoPresetsQueryResult> Handle(GetInteriorDesignerTourInfoPresetsQuery request, CancellationToken cancellationToken)
    {
        var result = 
            await _interiorDesignerContext.FindAsync<InteriorDesignerAggregate>(new InteriorDesignerIdentity(request.InteriorDesignerCode));

        var items = result.TourInfoPresets.Select(x => new TourInfoPresetsQueryResultItem
        {
            Name = x.Name,
            ExternalLink = x.ExternalLink,
            IsDefault = x.IsDefault

        }).ToList();

        return new GetInteriorDesignerTourInfoPresetsQueryResult
        {
            Items = items
        };
    }
}