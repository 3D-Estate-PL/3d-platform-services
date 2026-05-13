using InteriorDesigners.Application.Services;
using MediatR;

namespace InteriorDesigners.Application.InteriorsDesigners.Commands;

public class DeleteInteriorDesignerCommand : IRequest
{
    public string Code { get; set; }
}

public class DeleteInteriorDesignerCommandHandler : IRequestHandler<DeleteInteriorDesignerCommand, Unit>
{
    
    private readonly IInteriorDesignerRepository _interiorDesignerRepository;

    public DeleteInteriorDesignerCommandHandler(IInteriorDesignerRepository interiorDesignerRepository)
    {
        _interiorDesignerRepository = interiorDesignerRepository;
    }

    public async Task<Unit> Handle(DeleteInteriorDesignerCommand request, CancellationToken cancellationToken)
    {
        var interiorDesigner = await _interiorDesignerRepository.FindAsync(request.Code);
        await _interiorDesignerRepository.DeleteAsync(interiorDesigner);
        return Unit.Value;
    }
}