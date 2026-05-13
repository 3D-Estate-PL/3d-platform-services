namespace ImagesManagement.Api;

public class ConfigSettings
{
    public const string ComputerVisionEdnpoint = "ComputerVisionEdnpoint";
    public const string ComputerVisionKey = "ComputerVisionKey";


    public static string StorageKeyNameInSecretStore =>
        Environment.GetEnvironmentVariable("StorageKey") ?? "StorageKey";

    public static string StorageAccountNameInSecretStore =>
        Environment.GetEnvironmentVariable("StorageAccountName") ?? "StorageAccountName";

    public static string StorageContainerNameInSecretStore =>
        Environment.GetEnvironmentVariable("ContainerName") ?? "ContainerName";


    public static string SecretStoreName =>
        Environment.GetEnvironmentVariable("SecretStoreName") ?? "xeniel-dapr-secret-store";

 
    public static string TopicName => Environment.GetEnvironmentVariable("TopicName") ?? "products-images";

    public const string Route = "product-image-added";
    
}