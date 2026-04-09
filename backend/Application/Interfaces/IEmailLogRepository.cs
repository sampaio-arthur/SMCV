namespace SMCV.Application.Interfaces;

using SMCV.Domain.Entities;

public interface IEmailLogRepository : IRepository<EmailLog>
{
    Task<IEnumerable<EmailLog>> GetByContactIdAsync(Guid contactId);
    Task<IEnumerable<EmailLog>> GetByCampaignIdAsync(Guid campaignId);
}
