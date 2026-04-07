using FluentValidation;

namespace SMCV.Features.Contacts.Commands.CreateContact;

public class CreateContactCommandValidator : AbstractValidator<CreateContactCommand>
{
    public CreateContactCommandValidator()
    {
        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("CompanyName é obrigatório.")
            .MaximumLength(500);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório.")
            .EmailAddress().WithMessage("Formato de e-mail inválido.")
            .MaximumLength(255);

        RuleFor(x => x.Domain)
            .NotEmpty().WithMessage("Domain é obrigatório.")
            .MaximumLength(255);

        RuleFor(x => x.CampaignId)
            .NotEqual(Guid.Empty).WithMessage("CampaignId é obrigatório.");
    }
}
