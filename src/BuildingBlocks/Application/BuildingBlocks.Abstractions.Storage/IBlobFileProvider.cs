namespace BuildingBlocks.Abstractions.Storage;

public interface IBlobFileProvider
{
    Task<Stream> OpenReadAsync(string fileUrl);
    Task MoveAsync(string fileUrl, string folder, string suffixFile);
}