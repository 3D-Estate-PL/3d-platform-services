using BuildingBlocks.Domain.DDD;

namespace ImagesManagement.Domain;

public sealed record ImageResolutionEntity 
{
    public  int? Height { get; init; }
    public  int? Width { get; init; }
    
    public int? MaxRes { get; set; }

    public int Compression { get; init; }
    
    public Encoder Encoder { get; init; }

    public ImageResolutionEntity(int? height, int? width, int? maxRes, int comression, Encoder encoder)
    {
        if (maxRes.HasValue == false && (height.HasValue == false || width.HasValue == false))
        {
            throw new ArgumentException("Invalid argument. MaxRes or height or width are required.");
        }
        
        Width = width;
        Height = height;
        MaxRes = maxRes;
        Compression = comression;
        Encoder = encoder;
    }
    
    
    public string ImageSuffix()
    {
        if (MaxRes.HasValue)
        {
            return $"{MaxRes}";
        }
        else
        {
            return $"{Width}x{Height}";
        }
    }
}