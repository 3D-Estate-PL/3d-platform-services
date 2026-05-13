using BuildingBlocks.Domain.DDD;

namespace InteriorDesigners.Domain.InteriorDesigner;

public class InteriorDesignerIdentity : DocumentIdentity<InteriorDesignerAggregate>
{
    public InteriorDesignerIdentity(string id) : base(id)
    {
    }
}