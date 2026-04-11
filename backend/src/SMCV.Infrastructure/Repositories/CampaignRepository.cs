using Microsoft.EntityFrameworkCore;
using SMCV.Application.Interfaces;
using SMCV.Domain.Entities;
using SMCV.Infrastructure.Data;

namespace SMCV.Infrastructure.Repositories;

public class CampaignRepository : BaseRepository<Campaign>, ICampaignRepository
{
    public CampaignRepository(AppDbContext context) : base(context) { }

    public async Task<Campaign?> GetByIdWithContactsAsync(Guid id) =>
        await _context.Campaigns
            .Include(c => c.Contacts)
                .ThenInclude(ct => ct.EmailLog)
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<IEnumerable<Campaign>> GetAllWithContactsAsync() =>
        await _context.Campaigns
            .Include(c => c.Contacts)
            .ToListAsync();
}
