using BuildingBlocks.Application.Exceptions.Exceptions;
using InteriorDesigners.Application.Services;
using InteriorDesigners.Domain.InteriorDesigner;
using MediatR;

namespace InteriorDesigners.Application.InteriorsDesigners.Commands.Presets;

public class AddTourInfoPresetCommand : IRequest
{
    public string InteriorDesignerCode { get; set; }
    public string Name { get; set; }
    public string ExternalLink { get; set; }
    public bool IsDefault { get; set; }
}

public class AddTourInfoPresetCommandHandler : IRequestHandler<AddTourInfoPresetCommand, Unit>
{
    private readonly IInteriorDesignerRepository _interiorDesignerRepository;

    public AddTourInfoPresetCommandHandler(IInteriorDesignerRepository interiorDesignerRepository)
    {
        _interiorDesignerRepository = interiorDesignerRepository;
    }

    public async Task<Unit> Handle(AddTourInfoPresetCommand request, CancellationToken cancellationToken)
    {
        var interiorDesigner = await _interiorDesignerRepository.FindAsync(request.InteriorDesignerCode);

        if (interiorDesigner.TourInfoPresets.Any(x => x.IsDefault) && request.IsDefault)
        {
            interiorDesigner.TourInfoPresets.SingleOrDefault(x => x.IsDefault).IsDefault = false;
        }


        if (interiorDesigner.TourInfoPresets.Any(x => x.Name == request.Name))
        {
            throw new CustomException("Preset Name must be unique.");
        }

        interiorDesigner.TourInfoPresets.Add(TourInfoPreset.New(request.Name, request.ExternalLink, request.IsDefault));
        await _interiorDesignerRepository.UpsertAsync(interiorDesigner, cancellationToken);
        
        return Unit.Value;
    }
}