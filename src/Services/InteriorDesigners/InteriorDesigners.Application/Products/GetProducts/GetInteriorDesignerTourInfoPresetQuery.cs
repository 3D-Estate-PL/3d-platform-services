using MediatR;

namespace InteriorDesigners.Application.Products.GetProducts;

public class GetInteriorDesignerTourInfoPresetQuery : IRequest<GetInteriorDesignerTourInfoPresetQueryResult>
{
    public string InteriorDesignerCode { get; set; }
    public string Name { get; set; }
}

public class GetInteriorDesignerTourInfoPresetQueryResult
{
    public Dictionary<string,string> Data { get; set; }
}