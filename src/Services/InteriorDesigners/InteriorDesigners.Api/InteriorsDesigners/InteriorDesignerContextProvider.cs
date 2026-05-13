using System.Net.Http.Headers;

namespace InteriorDesigners.Api.InteriorsDesigners;

public static class InteriorDesignerContextProvider
{
    public static string GetInteriorDesignerContext(this HttpContext httpContext)
    {
        httpContext.Request.Headers.TryGetValue("Origin", out var origin);

        if (origin.ToString().Contains("decoroom", StringComparison.OrdinalIgnoreCase))
            return  "Decoroom"; 
        if (origin.ToString().Contains("configurator", StringComparison.OrdinalIgnoreCase))
            return  "3dEstate";
        if (origin.ToString().Contains("localhost", StringComparison.OrdinalIgnoreCase))
            return  "3dEstate";
        
        
        throw  new NotSupportedException("Not supported domain.");
    }

    public static string GetLanguageFromHeader(this HttpContext httpContext)
    {
        var languages = httpContext.Request.Headers["accept-language"].ToString().Split(',')
            .Select(StringWithQualityHeaderValue.Parse)
            .OrderByDescending(s => s.Quality.GetValueOrDefault(1));
        
        var language = languages.MaxBy(x=>x.Quality);
        if (language != null)
        {
            return language.Value.Split("-").First();
        }

        return null;
    }
}