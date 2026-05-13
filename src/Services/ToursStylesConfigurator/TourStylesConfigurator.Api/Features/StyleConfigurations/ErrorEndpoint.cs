namespace TourStylesConfigurator.Api.Features.StyleConfigurations;



public class ErrorEndpoint : Endpoint<EmptyRequest, EmptyResponse>
{
    public override void Configure()
    {
        AllowAnonymous();
        Get(
            $"/Errors");
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        throw new ArgumentException("Invalid parameters");
        await SendAsync(new EmptyResponse());
    }
}


public class LongRequestEndpoint : Endpoint<EmptyRequest, EmptyResponse>
{
    public override void Configure()
    {
        AllowAnonymous();
        Get(
            $"/Performance");
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        await Task.Delay(5000);
        await SendAsync(new EmptyResponse());
    }
}