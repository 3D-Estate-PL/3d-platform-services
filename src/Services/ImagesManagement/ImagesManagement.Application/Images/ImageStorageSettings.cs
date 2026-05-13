namespace ImagesManagement.Application.Images;

public class ImageStorageSettings
{
    public string ConfigFileUrl { get; set; }
    public List<StorageMap> Mapping { get; set; }
}

public class StorageMap
{
    public string Key { get; set; }
    public string StorageUrl { get; set; }
    public string ConnectionString { get; set; }
}