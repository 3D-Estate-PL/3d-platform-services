using MediatR;

namespace InteriorDesigners.Application.Products.GetProducts;

public class GetInteriorDesignerTourInfoPresetsQueryResult
{
    public List<TourInfoPresetsQueryResultItem> Items { get; set; } = new();
}

public class TourInfoPresetsQueryResultItem
{
    public string Name { get; set; }
    public string ExternalLink { get; set; }
    public bool IsDefault { get; set; }
}

public class GetInteriorDesignerTourInfoPresetsQuery : IRequest<GetInteriorDesignerTourInfoPresetsQueryResult>
{

    public string InteriorDesignerCode { get; set; }
}