using FluentValidation;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.ConfigurationAdd;

public class CreateNewConfigurationRequestValidator : Validator<CreateNewConfigurationRequest>
{
    public CreateNewConfigurationRequestValidator()
    {
        RuleFor(x => x.ConfigurationOwner.Name)
            .NotEmpty()
            .WithMessage("Nazwa jest wymagana")
            .MinimumLength(1)
            .WithMessage("Nazwa jest zbyt krótka (min 1 znak)")
            .MaximumLength(50)
            .WithMessage("Nazwa jest zbyt długa (max 50 znaków)");

        RuleFor(x => x.InvestmentName)
            .NotEmpty()
            .WithMessage("Nazwa jest wymagana")
            .MinimumLength(1)
            .WithMessage("Nazwa jest zbyt krótka (min 1 znak)")
            .MaximumLength(50)
            .WithMessage("Nazwa jest zbyt długa (max 50 znaków)");

    }
}