using FluentValidation;

namespace SMCV.Features.Campaigns.Commands.CreateCampaign;

public class CreateCampaignCommandValidator : AbstractValidator<CreateCampaignCommand>
{
    public CreateCampaignCommandValidator()
    {
        RuleFor(x => x.Niche)
            .NotEmpty().WithMessage("Niche é obrigatório.")
            .MaximumLength(255);

        RuleFor(x => x.Region)
            .NotEmpty().WithMessage("Region é obrigatório.")
            .MaximumLength(255);

        RuleFor(x => x.ResumeFileName)
            .NotEmpty().WithMessage("ResumeFileName é obrigatório.")
            .MaximumLength(500);

        RuleFor(x => x.ResumeFilePath)
            .NotEmpty().WithMessage("ResumeFilePath é obrigatório.")
            .MaximumLength(1000);

        RuleFor(x => x.EmailSubject)
            .NotEmpty().WithMessage("EmailSubject é obrigatório.")
            .MaximumLength(500);

        RuleFor(x => x.EmailBody)
            .NotEmpty().WithMessage("EmailBody é obrigatório.");
    }
}
