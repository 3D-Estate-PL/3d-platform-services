namespace TourStylesConfigurator.Api.Features.InteriorsDesigners.Products.GetProductDetailsV2;

public class InteriorDesignerProductDto
{
    public string Id { get; set; }
    
    public string InteriorDesigner { get; set; }
    
    public bool? IsDeleted { get; set; }

    public bool IsEnabled { get; set; }

    public string DisplayName { get; set; }

    public string Description { get; set; }
    
    public bool ShowLabel { get; set; }

    public SupplierDto Supplier { get; set; }
    public decimal? Price { get; set; }
}


public class SupplierDto
{
    public string? Name { get; set; }
    public string? Link { get; set; }
    public string? Logo { get; set; }
    public string? ProductLink { get; set; }
    public ContactDto? Contact { get; set; }
}

public class ContactDto
{
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
}