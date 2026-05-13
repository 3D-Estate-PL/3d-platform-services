using ImagesManagement.Domain;

namespace ImagesManagement.Application.Images;

public static class Helper
{
    public static string? GetDestinationFileName(string? destinationFileName, Encoder fileEncoder)
    {
        if (string.IsNullOrEmpty(destinationFileName) == false)
        {
            var destExtension = Path.GetExtension(destinationFileName);
            if (string.IsNullOrEmpty(destExtension))
            {
                destinationFileName = $"{destinationFileName}.{fileEncoder.ToString().ToLower()}";
                return destinationFileName;
            }
        }

        return null;
    }
    
    public static string GetExtension(string? destinationFileName, string imageName)
    {
        var extension = Path.GetExtension(destinationFileName);
        if (string.IsNullOrEmpty(extension))
        {
            extension = Path.GetExtension(imageName);
        }

        return extension;
    }
    
    public static Encoder GetDefaultEncoder(string fileExtension)
    {
        if (fileExtension?.ToUpper() == ".PNG")
        {
            return Encoder.PNG;
        }
        if (fileExtension?.ToUpper() == ".JPG")
        {
            return Encoder.JPG;
        }

        return Encoder.PNG;
    }
}