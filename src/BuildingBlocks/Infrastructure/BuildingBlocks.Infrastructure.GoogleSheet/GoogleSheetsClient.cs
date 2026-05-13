using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Microsoft.Extensions.Configuration;

namespace BuildingBlocks.Infrastructure.GoogleSheet;

public class GoogleSheetsClient
{
    private readonly SheetsService _service;
    private const string ApplicationName = "3dEstatePlatform";
    private static readonly string[] Scopes = {SheetsService.Scope.Spreadsheets};
    private readonly IConfiguration _configuration;


    public GoogleSheetsClient(IConfiguration configuration)
    {
        _configuration = configuration;
        var credential = GetCredentialsFromFile();
        _service = new SheetsService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName
        });
    }

    public SheetsService Get()
    {
        return _service;
    }

    private GoogleCredential GetCredentialsFromFile()
    {
        var credential = GoogleCredential.FromJson(_configuration["GoogleCredentials"]);
        return credential.CreateScoped(Scopes);
    }
}