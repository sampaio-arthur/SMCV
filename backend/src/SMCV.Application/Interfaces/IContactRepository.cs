namespace SMCV.Application.Interfaces;

using SMCV.Domain.Entities;

public interface IContactRepository : IRepository<Contact>
{
    Task<IEnumerable<Contact>> GetByCampaignIdAsync(Guid campaignId);
    Task<Contact?> GetByIdWithEmailLogAsync(Guid id);
    Task<Contact?> GetByEmailAsync(string email, Guid campaignId);
}
