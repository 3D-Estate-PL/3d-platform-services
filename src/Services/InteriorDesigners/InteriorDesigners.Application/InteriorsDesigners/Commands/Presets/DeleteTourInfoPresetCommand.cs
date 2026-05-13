using InteriorDesigners.Application.Services;
using MediatR;

namespace InteriorDesigners.Application.InteriorsDesigners.Commands.Presets;

public class DeleteTourInfoPresetCommand : IRequest
{
    public string InteriorDesignerCode { get; set; }
    public string Name { get; set; }
}

public class DeleteTourInfoPresetCommandHandler : IRequestHandler<DeleteTourInfoPresetCommand, Unit>
{
    
    private readonly IInteriorDesignerRepository _interiorDesignerRepository;

    public DeleteTourInfoPresetCommandHandler(IInteriorDesignerRepository interiorDesignerRepository)
    {
        _interiorDesignerRepository = interiorDesignerRepository;
    }

    public async Task<Unit> Handle(DeleteTourInfoPresetCommand request, CancellationToken cancellationToken)
    {
        var interiorDesigner = await _interiorDesignerRepository.FindAsync(request.InteriorDesignerCode);
        var preset = interiorDesigner.TourInfoPresets.SingleOrDefault(x => x.Name == request.Name);
        interiorDesigner.TourInfoPresets.Remove(preset);
        
        await _interiorDesignerRepository.UpsertAsync(interiorDesigner, cancellationToken);

        return Unit.Value;
    }
}