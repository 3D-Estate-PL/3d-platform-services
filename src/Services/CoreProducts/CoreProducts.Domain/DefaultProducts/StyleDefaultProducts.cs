using BuildingBlocks.Domain.DDD;

namespace CoreProducts.Domain.DefaultProducts;

public class StyleDefaultProducts : IDocument
{
    /// <summary>
    /// Style Code
    /// </summary>
    public required string Code { get; init; }

    /// <summary>
    /// Rooms
    /// </summary>
    public List<Room> RoomTypes { get; set; } = new List<Room>();

    public DocumentIdentity GetIdentity()
    {
        return new StyleDefaultProductsIdentity(Code);
    }
}

public class Room
{
    /// <summary>
    /// Room Type
    /// </summary>
    public required string Type { get; init; }
    /// <summary>
    /// Default Products
    /// </summary>
    public required List<DefaultProduct> DefaultProducts { get; set; } = new List<DefaultProduct>();
}
public class DefaultProduct
{
    /// <summary>
    /// Category Name
    /// </summary>
    public required string CategoryName { get; init; }
    /// <summary>
    /// ProductId
    /// </summary>
    public required string ProductId { get; init; }
}
