namespace SMCV.Application.Interfaces;

using SMCV.Domain.Entities;

public interface ICampaignRepository : IRepository<Campaign>
{
    Task<Campaign?> GetByIdWithContactsAsync(Guid id);
    Task<IEnumerable<Campaign>> GetAllWithContactsAsync();
}
