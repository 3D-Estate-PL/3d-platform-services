using System.Text.Json;

namespace TourStylesConfigurator.Api.Features.Dictionaries.GetCategories;

public class RoomCategoryDto
{
    public string RoomType { get; set; }
    public CategoryDto Category { get; set; }
}

public class CategoryDto
{
    private string _categoryName;

    public string Name
    {
        get => _categoryName;
        set => _categoryName = JsonNamingPolicy.CamelCase.ConvertName(value);
    }

    public string GetNameWithRoomPrefix(string roomType)
    {
        return $"{roomType}.{Name}";
    }

    public string? DisplayName { get; set; }
}