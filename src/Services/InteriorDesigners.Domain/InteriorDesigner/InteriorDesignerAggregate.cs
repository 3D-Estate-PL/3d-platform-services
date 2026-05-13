using System.Text.Json;
using System.Text.Json.Serialization;
using BuildingBlocks.Domain.DDD;
using Newtonsoft.Json;

namespace InteriorDesigners.Domain.InteriorDesigner;

public class InteriorDesignerAggregate : IDocument
{
    [JsonProperty("code")]
    public string Code { get; private set; }
    
    public string DisplayName { get;  set; }
    
    [JsonProperty("spreadSheetId")]
    public string SpreadSheetId { get; private set; }
    
    [JsonProperty("sheetName")]
    public string SheetName { get; private set; }
    
    [JsonProperty("productsExternalLink")]
    public string ProductsExternalLink { get; private set; }

    public List<TourInfoPreset> TourInfoPresets { get; set; } =
        new List<TourInfoPreset>();

    public List<TourStyle> Styles { get; set; } =  new List<TourStyle>();


    public InteriorDesignerAggregate()
    {
        
    }
    
    public static InteriorDesignerAggregate New(string code, string displayName,string productsExternalLink)
    {
        var aggregate = new InteriorDesignerAggregate
        {
            Code = code,
            DisplayName = displayName,
            SheetName = "Products"
        };
        aggregate.SetProductExternalLink(productsExternalLink);

        return aggregate;
    }

    public void SetProductExternalLink(string productExternalLink)
    {
        ProductsExternalLink = productExternalLink;
        SpreadSheetId = new Uri(productExternalLink).PathAndQuery.Split("/")[3];
    }
    
    public DocumentIdentity GetIdentity()
    {
        return new InteriorDesignerIdentity(Code);
    }

    public void RemoveStyles()
    {
        Styles.Clear();
    }
}