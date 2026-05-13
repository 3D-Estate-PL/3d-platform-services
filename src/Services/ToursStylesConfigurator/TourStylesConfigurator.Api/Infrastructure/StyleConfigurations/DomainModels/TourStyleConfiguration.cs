using BuildingBlocks.Domain.DDD;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels.Styles;

namespace TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels;

public class TourStyleConfiguration : IDocument
{
    public string Id { get; set; }
    public string Code { get; set; }
    public string OwnerId { get; set; }
    public string InteriorDesignerCode { get; set; }

    public StyleConfigurationStatus Status { get; set; }
    public string InvestmentName { get; set; }
    
    public bool IsEditable { get; set; }

    public Place ExteriorStyleConfiguration { get; set; }

    public Place InteriorStyleConfiguration { get; set; }
    public DateTime? CreatedDate { get; set; }

    public Place GetPlace(PlaceType placeType)
    {
        return placeType switch
        {
            PlaceType.Interior => InteriorStyleConfiguration,
            PlaceType.Exterior => ExteriorStyleConfiguration,
            _ => throw new InvalidOperationException()
        };
    }

    public static TourStyleConfiguration New(string ownerId, string investmentName, string interiorDesignerCode)
    {
        var exteriorStyleConfigurationItem = StyleConfigurationItem.Exterior();

        var roomItem = RoomItem.New("default", "exterior", RoomStyle.New(new TourStyle
        {
        }), new List<ProductItem>(), "default");

        exteriorStyleConfigurationItem.AddRoom(roomItem);

        var styleConfiguration = new TourStyleConfiguration
        {
            Id = Guid.NewGuid().ToString(),
            CreatedDate = DateTime.UtcNow,
            IsEditable = true,
            OwnerId = ownerId,
            InvestmentName = investmentName,
            Status = StyleConfigurationStatus.Draft,
            Code = RandomString(5),
            InteriorDesignerCode = interiorDesignerCode,
            InteriorStyleConfiguration = new Place
            {
                Styles = new List<StyleConfigurationItem>()
            },
            ExteriorStyleConfiguration = new Place
            {
                Styles = new List<StyleConfigurationItem>
                {
                    exteriorStyleConfigurationItem
                }
            }
        };
        return styleConfiguration;
    }


    private static readonly Random random = new();

    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }


    public DocumentIdentity GetIdentity()
    {
        return new TourStyleConfigurationIdentity(Id);
    }

    public void SubmitOrder()
    {
        Status = StyleConfigurationStatus.AwaitingForAcceptation;
        IsEditable = false;
    }

    public TourStyleConfiguration Clone()
    {
        var newVersion = New(OwnerId, InvestmentName, InteriorDesignerCode);

        newVersion.InteriorStyleConfiguration.SetStyles(InteriorStyleConfiguration.Clone());
        newVersion.ExteriorStyleConfiguration.SetStyles(ExteriorStyleConfiguration.Clone());

        return newVersion;
    }
}