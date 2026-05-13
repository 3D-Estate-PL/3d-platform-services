using InteriorDesigners.Domain.InteriorDesigner;

namespace _3dEstate.Platform.Services.InteriorDesigners.Features.InteriorsDesigners.GetAll;

public class InteriorDesignerDto
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string SheetName { get; set; }
    public string ProductsExternalLink { get; set; }
    public List<TourStyle> Styles { get; set; }
}