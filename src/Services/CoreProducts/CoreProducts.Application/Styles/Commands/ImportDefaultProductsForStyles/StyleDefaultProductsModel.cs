using CoreProducts.Domain.DefaultProducts;

namespace CoreProducts.Application.Styles.Commands.ImportDefaultProductsForStyles;

public class StyleDefaultProductsModel
{
    public string Code { get; set; }
    public List<RoomDefaultProduct> Rooms { get; set; }

    public StyleDefaultProducts MapToDomainModel(StyleDefaultProducts styleDefaultProducts)
    {
        styleDefaultProducts.RoomTypes = new List<Room>();
        foreach (var room in Rooms)
        {
            var item = new Room
            {
                Type = room.Room,
                DefaultProducts = new List<DefaultProduct>()
            };

            foreach (var defaultCategoryProduct in room.Elements)
            {
                item.DefaultProducts.Add(new DefaultProduct
                {
                    ProductId = defaultCategoryProduct.Id,
                    CategoryName = item.Type+"."+defaultCategoryProduct.CategoryName.Split('.').Last()
                });
            }
            
            styleDefaultProducts.RoomTypes.Add(item);
        }

        return styleDefaultProducts;
    }
}

public class RoomDefaultProduct
{
    public string Room { get; set; }
    public List<DefaultCategoryProduct> Elements { get; set; }
}

public class DefaultCategoryProduct
{
    public string CategoryName { get; set; }
    public string Id { get; set; }
}