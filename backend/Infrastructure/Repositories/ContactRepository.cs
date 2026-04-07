using Microsoft.EntityFrameworkCore;
using SMCV.Application.Interfaces;
using SMCV.Domain.Entities;
using SMCV.Infrastructure.Data;

namespace SMCV.Infrastructure.Repositories;

public class ContactRepository : BaseRepository<Contact>, IContactRepository
{
    public ContactRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Contact>> GetByCampaignIdAsync(Guid campaignId) =>
        await _context.Contacts
            .Include(c => c.EmailLog)
            .Where(c => c.CampaignId == campaignId)
            .ToListAsync();

    public async Task<Contact?> GetByIdWithEmailLogAsync(Guid id) =>
        await _context.Contacts
            .Include(c => c.EmailLog)
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<Contact?> GetByEmailAsync(string email, Guid campaignId) =>
        await _context.Contacts
            .FirstOrDefaultAsync(c => c.Email == email && c.CampaignId == campaignId);
}
