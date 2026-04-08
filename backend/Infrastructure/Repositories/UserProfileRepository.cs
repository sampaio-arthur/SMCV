using Microsoft.EntityFrameworkCore;
using SMCV.Application.Interfaces;
using SMCV.Domain.Entities;
using SMCV.Infrastructure.Data;

namespace SMCV.Infrastructure.Repositories;

public class UserProfileRepository : BaseRepository<UserProfile>, IUserProfileRepository
{
    public UserProfileRepository(AppDbContext context) : base(context) { }

    public async Task<UserProfile?> GetByUserIdAsync(Guid userId) =>
        await _context.UserProfiles.FirstOrDefaultAsync(up => up.UserId == userId);
}
