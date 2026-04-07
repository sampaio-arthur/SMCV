using FluentValidation;

namespace SMCV.Features.Campaigns.Commands.UpdateCampaign;

public class UpdateCampaignCommandValidator : AbstractValidator<UpdateCampaignCommand>
{
    public UpdateCampaignCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty).WithMessage("Id é obrigatório.");

        RuleFor(x => x.EmailSubject)
            .NotEmpty().WithMessage("EmailSubject é obrigatório.")
            .MaximumLength(500);

        RuleFor(x => x.EmailBody)
            .NotEmpty().WithMessage("EmailBody é obrigatório.");
    }
}
