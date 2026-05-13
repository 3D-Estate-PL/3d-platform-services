using System.Text.Json;
using BuildingBlocks.Abstractions.CQRS.CQRS;
using BuildingBlocks.Abstractions.Storage;
using BuildingBlocks.Application.Exceptions.Exceptions;
using CoreProducts.Application.Styles.Services;
using CoreProducts.Domain.DefaultProducts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CoreProducts.Application.Styles.Commands.ImportDefaultProductsForStyles;

public class ImportDefaultStyleProductsFromFileCommand : ICommand
{
    public string FileUrl { get; }

    public ImportDefaultStyleProductsFromFileCommand(string fileUrl)
    {
        FileUrl = fileUrl;
    }
}

internal class
    ImportDefaultStyleProductsFromFileCommandHandler : ICommandHandler<ImportDefaultStyleProductsFromFileCommand>
{
    private readonly ILogger<ImportDefaultStyleProductsFromFileCommandHandler> _logger;
    private readonly IStyleDefaultProductsRepository _styleDefaultProductsRepository;
    private readonly IBlobFileProvider _blobFileProvider;


    public ImportDefaultStyleProductsFromFileCommandHandler(
        ILogger<ImportDefaultStyleProductsFromFileCommandHandler> logger,
        IStyleDefaultProductsRepository styleDefaultProductsRepository,
        IBlobFileProvider blobFileProvider)
    {
        _logger = logger;
        _styleDefaultProductsRepository = styleDefaultProductsRepository;
        _blobFileProvider = blobFileProvider;
    }

    public async Task<Unit> Handle(ImportDefaultStyleProductsFromFileCommand request,
        CancellationToken cancellationToken)
    {
        if (request.FileUrl.Contains("Imported"))
        {
            return Unit.Value;
        }
        
        StyleDefaultProductsModel? itemToImport = null;

        await using (var stream = await _blobFileProvider.OpenReadAsync(request.FileUrl))
        {
            itemToImport =
                await JsonSerializer.DeserializeAsync<StyleDefaultProductsModel>(stream,
                    cancellationToken: cancellationToken, options: new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
        }

        if (itemToImport == null) throw new CustomException("Invalid file with style default products.");


        await ImportDefaultStyleProducts(itemToImport, request.FileUrl);


        await _blobFileProvider.MoveAsync(request.FileUrl, "Imported", $"{DateTime.UtcNow:yyyy_MM_dd_HH_mm}");
        return Unit.Value;
    }

    private async Task ImportDefaultStyleProducts(StyleDefaultProductsModel styleDefaultProductsModel, string fileName)
    {
        var entity = new StyleDefaultProducts
        {
            Code = Path.GetFileNameWithoutExtension(fileName)
        };

        styleDefaultProductsModel.MapToDomainModel(entity);
        await _styleDefaultProductsRepository.UpsertAsync(entity, new CancellationToken());
        _logger.LogInformation("Imported: {Code}", entity.Code);
    }
}