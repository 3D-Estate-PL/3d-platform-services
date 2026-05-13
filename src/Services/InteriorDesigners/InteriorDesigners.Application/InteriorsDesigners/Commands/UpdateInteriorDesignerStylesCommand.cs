using InteriorDesigners.Application.Services;
using InteriorDesigners.Domain.InteriorDesigner;
using MediatR;

namespace InteriorDesigners.Application.InteriorsDesigners.Commands;

public class UpdateInteriorDesignerStylesCommand : IRequest
{
    public List<TourStyle> Styles { get; set; }
    public string Code { get; set; }
}

public class UpdateInteriorDesignerStylesCommandHandler : IRequestHandler<UpdateInteriorDesignerStylesCommand, Unit>
{
    private readonly IInteriorDesignerRepository _interiorDesignerRepository;

    public UpdateInteriorDesignerStylesCommandHandler(IInteriorDesignerRepository interiorDesignerRepository)
    {
        _interiorDesignerRepository = interiorDesignerRepository;
    }

    public async Task<Unit> Handle(UpdateInteriorDesignerStylesCommand request, CancellationToken cancellationToken)
    {
        var interiorDesigner = await _interiorDesignerRepository.FindAsync(request.Code);
        interiorDesigner.RemoveStyles();
        foreach (var style in request.Styles)
        {
            var tourStyle =new TourStyle
            {
                Group = style.Group,
                Kind = style.Kind
            };
            interiorDesigner.Styles.Add(tourStyle);
        }

        await _interiorDesignerRepository.UpsertAsync(interiorDesigner, cancellationToken);
        return Unit.Value;
    }
}