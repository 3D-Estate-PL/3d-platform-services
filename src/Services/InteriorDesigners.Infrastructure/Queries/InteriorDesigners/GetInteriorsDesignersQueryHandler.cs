using _3dEstate.Platform.Services.InteriorDesigners.Features.InteriorsDesigners.GetAll;
using BuildingBlocks.Abstractions.CQRS.CQRS;
using BuildingBlocks.Infrastructure.DataAccess.Cosmos.Extensions;
using InteriorDesigners.Application.InteriorsDesigners.Queries.GetAll;
using InteriorDesigners.Domain.InteriorDesigner;
using InteriorDesigners.Infrastructure.DataAccess;

namespace InteriorDesigners.Infrastructure.Queries.InteriorDesigners;

internal class GetInteriorsDesignersQueryHandler : IQueryHandler<GetInteriorsDesignersQuery, GetInteriorsDesignersQueryResponse>
{
    private readonly InteriorDesignerContext _interiorDesignerContext;

    public GetInteriorsDesignersQueryHandler(InteriorDesignerContext interiorDesignerContext)
    {
        _interiorDesignerContext = interiorDesignerContext;
    }

    public async Task<GetInteriorsDesignersQueryResponse> Handle(GetInteriorsDesignersQuery request, CancellationToken cancellationToken)
    {
        var result = (await _interiorDesignerContext.Query<InteriorDesignerAggregate>().ToDocumentListAsync()).Result;

        var productsModel = result.Select(entity => new InteriorDesignerDto
            {
                Code = entity.Code,
                Name = entity.DisplayName,
                SheetName = entity.SheetName,
                ProductsExternalLink = entity.ProductsExternalLink,
                Styles = entity.Styles.ToList()
            })
            .ToList();

        return new GetInteriorsDesignersQueryResponse(productsModel);
    }
}