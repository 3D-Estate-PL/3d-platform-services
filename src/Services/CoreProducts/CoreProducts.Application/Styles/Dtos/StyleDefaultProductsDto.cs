namespace CoreProducts.Application.Styles.Dtos;

public class StyleDefaultProductsDto
{
    /// <summary>
    /// Style Code
    /// </summary>
    public required string Code { get; init; }

    /// <summary>
    /// Rooms
    /// </summary>
    public List<RoomDto> RoomTypes { get; set; } = new List<RoomDto>();
}

public class RoomDto
{
    /// <summary>
    /// Room Type
    /// </summary>
    public required string Type { get; init; }
    /// <summary>
    /// Default Products
    /// </summary>
    public required List<DefaultProductDto> DefaultProducts { get; set; } = new List<DefaultProductDto>();
}
public class DefaultProductDto
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
