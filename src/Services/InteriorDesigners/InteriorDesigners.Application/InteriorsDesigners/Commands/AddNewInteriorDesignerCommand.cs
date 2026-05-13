using BuildingBlocks.Abstractions.CQRS.CQRS;
using InteriorDesigners.Application.Services;
using InteriorDesigners.Domain.InteriorDesigner;
using MediatR;

namespace InteriorDesigners.Application.InteriorsDesigners.Commands;

public class AddNewInteriorDesignerCommand : ICommand
{
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string ProductsExternalLink { get; set; }
}

internal class AddNewInteriorDesignerCommandHandler : ICommandHandler<AddNewInteriorDesignerCommand>
{
    private readonly IInteriorDesignerRepository _interiorDesignerRepository;


    public AddNewInteriorDesignerCommandHandler(IInteriorDesignerRepository interiorDesignerRepository)
    {
        _interiorDesignerRepository = interiorDesignerRepository;
    }

    public async Task<Unit> Handle(AddNewInteriorDesignerCommand request, CancellationToken cancellationToken)
    {
        var interiorDesigner = InteriorDesignerAggregate.New(request.Name,request.DisplayName,request.ProductsExternalLink);

        await _interiorDesignerRepository.UpsertAsync(interiorDesigner, cancellationToken);
        return Unit.Value;
    }
    
}