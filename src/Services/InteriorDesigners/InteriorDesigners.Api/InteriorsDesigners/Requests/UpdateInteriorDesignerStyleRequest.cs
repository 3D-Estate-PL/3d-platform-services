using InteriorDesigners.Domain.InteriorDesigner;

namespace InteriorDesigners.Api.InteriorsDesigners.Requests;

public class UpdateInteriorDesignerStyleRequest
{
    public List<TourStyle> Styles { get; set; }
}