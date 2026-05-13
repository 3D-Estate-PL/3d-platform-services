namespace InteriorDesigners.Api.InteriorsDesigners.Requests;

public class AddTourInfoPresetRequest
{
    public string Name { get; set; }
    public string ExternalLink { get; set; }
    public bool IsDefault { get; set; }
}