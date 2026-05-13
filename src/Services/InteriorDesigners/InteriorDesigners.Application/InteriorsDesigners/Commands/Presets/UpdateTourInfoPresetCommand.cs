using InteriorDesigners.Application.Services;
using MediatR;

namespace InteriorDesigners.Application.InteriorsDesigners.Commands.Presets;

public class UpdateTourInfoPresetCommand : IRequest
{
    public string InteriorDesignerCode { get; set; }
    public string Name { get; set; }
    public bool IsDefault { get; set; }
    
    public string ExternalLink { get; set; }
}

public class UpdateTourInfoPresetCommandHandler : IRequestHandler<UpdateTourInfoPresetCommand, Unit>
{
    private readonly IInteriorDesignerRepository _interiorDesignerRepository;


    public UpdateTourInfoPresetCommandHandler(IInteriorDesignerRepository interiorDesignerRepository)
    {
        _interiorDesignerRepository = interiorDesignerRepository;
    }

    public async Task<Unit> Handle(UpdateTourInfoPresetCommand request, CancellationToken cancellationToken)
    {
        var interiorDesigner = await _interiorDesignerRepository.FindAsync(request.InteriorDesignerCode);
        var preset = interiorDesigner.TourInfoPresets.SingleOrDefault(x => x.Name == request.Name);
        
        if (interiorDesigner.TourInfoPresets.Any(x => x.IsDefault) && request.IsDefault)
        {
            interiorDesigner.TourInfoPresets.SingleOrDefault(x => x.IsDefault).IsDefault = false;
        }
        
        preset.IsDefault = request.IsDefault;
        preset.SetExternalLink(request.ExternalLink);
        await _interiorDesignerRepository.UpsertAsync(interiorDesigner, cancellationToken);

       return Unit.Value;
    }
}