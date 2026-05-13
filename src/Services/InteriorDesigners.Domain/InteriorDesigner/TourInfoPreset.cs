using System.Dynamic;
using Newtonsoft.Json;

namespace InteriorDesigners.Domain.InteriorDesigner;

public class TourInfoPreset
{
    [JsonProperty("externalLink")]
    public string ExternalLink { get; private set; } 
    public string Name { get; set; }
    public bool IsDefault { get; set; }
    
    [JsonProperty("spreadSheetId")]
    public string SpreadSheetId { get; private set; }

    public Dictionary<string,string> Data { get; set; }

    public static TourInfoPreset New(string name, string externalLink,bool isDefault)
    {
        var aggregate = new TourInfoPreset
        {
            Name = name,
            IsDefault = isDefault
        };
        aggregate.SetExternalLink(externalLink);
        
        return aggregate;
    }
    
    public void SetExternalLink(string externalLink)
    {
        ExternalLink = externalLink;
        SpreadSheetId = new Uri(externalLink).PathAndQuery.Split("/")[3];
    }
    
    public TourInfoPreset()
    {}
    
}

public class Dynamic
{
}