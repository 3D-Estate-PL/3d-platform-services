using BuildingBlocks.Infrastructure.DataAccess.Cosmos.Extensions;
using InteriorDesigners.Application.Products.GetProducts;
using InteriorDesigners.Domain.InteriorDesigner;
using InteriorDesigners.Infrastructure.DataAccess;
using MediatR;
using Newtonsoft.Json;

namespace InteriorDesigners.Infrastructure.Queries.InteriorDesigners;

public class GetInteriorDesignerTourInfoPresetQueryHandler : IRequestHandler<GetInteriorDesignerTourInfoPresetQuery,GetInteriorDesignerTourInfoPresetQueryResult>
{
    private readonly InteriorDesignerContext _interiorDesignerContext;


    public GetInteriorDesignerTourInfoPresetQueryHandler(InteriorDesignerContext interiorDesignerContext)
    {
        _interiorDesignerContext = interiorDesignerContext;
    }

    public async Task<GetInteriorDesignerTourInfoPresetQueryResult> Handle(GetInteriorDesignerTourInfoPresetQuery request, CancellationToken cancellationToken)
    {
        var result = 
            await _interiorDesignerContext.FindAsync<InteriorDesignerAggregate>(new InteriorDesignerIdentity(request.InteriorDesignerCode));

        var preset = result.TourInfoPresets.SingleOrDefault(x => x.Name == request.Name);

        return new GetInteriorDesignerTourInfoPresetQueryResult
        {
            Data = preset.Data
        };
    }
}