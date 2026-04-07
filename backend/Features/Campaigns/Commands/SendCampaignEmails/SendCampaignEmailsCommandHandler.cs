using MediatR;
using SMCV.Application.Interfaces;
using SMCV.Common.Exceptions;
using SMCV.Domain.Entities;
using SMCV.Domain.Enums;

namespace SMCV.Features.Campaigns.Commands.SendCampaignEmails;

public class SendCampaignEmailsCommandHandler : IRequestHandler<SendCampaignEmailsCommand, int>
{
    private readonly ICampaignRepository _campaignRepository;
    private readonly IEmailLogRepository _emailLogRepository;
    private readonly IEmailSenderService _emailSenderService;

    public SendCampaignEmailsCommandHandler(
        ICampaignRepository campaignRepository,
        IEmailLogRepository emailLogRepository,
        IEmailSenderService emailSenderService)
    {
        _campaignRepository = campaignRepository;
        _emailLogRepository = emailLogRepository;
        _emailSenderService = emailSenderService;
    }

    public async Task<int> Handle(SendCampaignEmailsCommand request, CancellationToken cancellationToken)
    {
        var campaign = await _campaignRepository.GetByIdWithContactsAsync(request.CampaignId)
            ?? throw new NotFoundException("Campaign", request.CampaignId);

        campaign.Status = CampaignStatus.Running;
        await _campaignRepository.UpdateAsync(campaign);

        var successCount = 0;

        foreach (var contact in campaign.Contacts)
        {
            var existingLog = await _emailLogRepository.GetByContactIdAsync(contact.Id);
            if (existingLog is not null && existingLog.Status == EmailStatus.Sent)
                continue;

            var emailLog = new EmailLog
            {
                ContactId = contact.Id
            };

            try
            {
                await _emailSenderService.SendEmailWithAttachmentAsync(
                    contact.Email,
                    contact.ContactName ?? contact.CompanyName,
                    campaign.EmailSubject,
                    campaign.EmailBody,
                    campaign.ResumeFilePath,
                    campaign.ResumeFileName);

                emailLog.Status = EmailStatus.Sent;
                emailLog.SentAt = DateTime.UtcNow;
                successCount++;
            }
            catch (Exception ex)
            {
                emailLog.Status = EmailStatus.Failed;
                emailLog.ErrorMessage = ex.Message;
            }

            await _emailLogRepository.AddAsync(emailLog);
        }

        campaign.Status = CampaignStatus.Completed;
        await _campaignRepository.UpdateAsync(campaign);

        return successCount;
    }
}
