using _3dEstate.Platform.Services.InteriorDesigners.Features.InteriorsDesigners.GetAll;
using BuildingBlocks.Abstractions.CQRS.CQRS;

namespace InteriorDesigners.Application.InteriorsDesigners.Queries.GetAll;

public class GetInteriorsDesignersQueryResponse
{
    public GetInteriorsDesignersQueryResponse(List<InteriorDesignerDto> items)
    {
        Items = items;
    }

    public List<InteriorDesignerDto> Items { get; }
}

public class GetInteriorsDesignersQuery : IQuery<GetInteriorsDesignersQueryResponse>
{
}