using InteriorDesigners.Application.Services;
using MediatR;

namespace InteriorDesigners.Application.InteriorsDesigners.Commands;

public class UpdateInteriorDesignerCommand : IRequest
{
    public string Code { get; set; }
    public string DisplayName { get; set; }
    public string ProductExternalLink { get; set; }
}

public class UpdateInteriorDesignerCommandHandler : IRequestHandler<UpdateInteriorDesignerCommand, Unit>
{
    private readonly IInteriorDesignerRepository _interiorDesignerRepository;

    public UpdateInteriorDesignerCommandHandler(IInteriorDesignerRepository interiorDesignerRepository)
    {
        _interiorDesignerRepository = interiorDesignerRepository;
    }

    public async Task<Unit> Handle(UpdateInteriorDesignerCommand request, CancellationToken cancellationToken)
    {
        var interiorDesigner = await _interiorDesignerRepository.FindAsync(request.Code);
        interiorDesigner.DisplayName = request.DisplayName;
        interiorDesigner.SetProductExternalLink(request.ProductExternalLink);
        await _interiorDesignerRepository.UpsertAsync(interiorDesigner, cancellationToken);
        
        return Unit.Value;
    }
}