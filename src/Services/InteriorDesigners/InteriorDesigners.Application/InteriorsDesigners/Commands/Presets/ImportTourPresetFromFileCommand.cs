using BuildingBlocks.Abstractions.Excel;
using InteriorDesigners.Application.Services;
using MediatR;

namespace InteriorDesigners.Application.InteriorsDesigners.Commands.Presets;

public class ImportTourPresetFromFileCommand : IRequest
{
    public string InteriorDesignerCode { get; set; }
    public string Name { get; set; }
}


public class ImportTourPresetFromFileCommandHandler : IRequestHandler<ImportTourPresetFromFileCommand, Unit>
{
    private readonly IInteriorDesignerRepository _interiorDesignerRepository;
    private readonly IExcelDictionaryProvider _excelDictionaryProvider;

    public ImportTourPresetFromFileCommandHandler(IInteriorDesignerRepository interiorDesignerRepository, IExcelDictionaryProvider excelDictionaryProvider)
    {
        _interiorDesignerRepository = interiorDesignerRepository;
        _excelDictionaryProvider = excelDictionaryProvider;
    }

    public async Task<Unit> Handle(ImportTourPresetFromFileCommand request, CancellationToken cancellationToken)
    {
        var aggregate = await _interiorDesignerRepository.FindAsync(request.InteriorDesignerCode);
        var preset = aggregate.TourInfoPresets.Single(x => x.Name == request.Name);
        preset.Data.Clear();
        
        var gsp = new SheetParameters()
        {
            RangeColumnStart = 1,
            RangeRowStart = 1, 
            RangeColumnEnd = 20,
            RangeRowEnd = 3,
            SpreadSheetId = preset.SpreadSheetId
        };

        var result = await _excelDictionaryProvider.Import("Presets", gsp);

        var presetData = new Dictionary<string, string>();
        foreach (var item in result)
        {
            foreach (var property in item)
            {
                var propertyName = property.Key;
                var propertyValue = property.Value;
                presetData.Add(propertyName,propertyValue?.ToString());
            }

        }

        preset.Data = presetData;
        await _interiorDesignerRepository.UpsertAsync(aggregate, cancellationToken);

        return Unit.Value;
    }
}