namespace TourStylesConfigurator.Api.Features.StyleConfigurations.GetConfigurationStyles;

public static class StringExtensions
{
    public static string ToPascalCase(this string s)
    {
        var words = s.Split(new[] { '-', '_' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(word => word.Substring(0, 1).ToUpper()+
                            word.Substring(1).ToLower());

        var result = String.Concat(words);
        return result;    }
}