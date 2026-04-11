using MediatR;
using SMCV.Application.Interfaces;
using SMCV.Common.Exceptions;
using SMCV.Domain.Enums;

namespace SMCV.Features.Campaigns.Commands.DeleteCampaign;

public class DeleteCampaignCommandHandler : IRequestHandler<DeleteCampaignCommand>
{
    private readonly ICampaignRepository _campaignRepository;

    public DeleteCampaignCommandHandler(ICampaignRepository campaignRepository)
    {
        _campaignRepository = campaignRepository;
    }

    public async Task Handle(DeleteCampaignCommand request, CancellationToken cancellationToken)
    {
        var campaign = await _campaignRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException("Campaign", request.Id);

        if (campaign.Status == CampaignStatus.Running)
            throw new InvalidOperationException("Campanha em execução não pode ser deletada.");

        await _campaignRepository.DeleteAsync(campaign);
    }
}
