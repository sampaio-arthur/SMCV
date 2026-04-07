using Microsoft.EntityFrameworkCore;
using SMCV.Application.Interfaces;
using SMCV.Domain.Entities;
using SMCV.Infrastructure.Data;

namespace SMCV.Infrastructure.Repositories;

public class EmailLogRepository : BaseRepository<EmailLog>, IEmailLogRepository
{
    public EmailLogRepository(AppDbContext context) : base(context) { }

    public async Task<EmailLog?> GetByContactIdAsync(Guid contactId) =>
        await _context.EmailLogs
            .FirstOrDefaultAsync(el => el.ContactId == contactId);

    public async Task<IEnumerable<EmailLog>> GetByCampaignIdAsync(Guid campaignId) =>
        await _context.EmailLogs
            .Include(el => el.Contact)
            .Where(el => el.Contact.CampaignId == campaignId)
            .ToListAsync();
}
