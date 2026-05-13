namespace TourStylesConfigurator.Api.Infrastructure.InteriorsDesigners.Products;

public class StyleDefaultProducts
{
    public string Code { get; set; }

    public List<Room> RoomTypes { get; set; }
}

public class Room
{
    public string Type { get; set; }
    public List<DefaultProduct> DefaultProducts { get; set; }

    public Room()
    {
        DefaultProducts = new List<DefaultProduct>();
    }
}

public class DefaultProduct
{
    public string CategoryName { get; set; }
    public string ProductId { get; set; }
}