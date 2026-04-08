using Microsoft.EntityFrameworkCore;
using SMCV.Application.Interfaces;
using SMCV.Domain.Entities;
using SMCV.Infrastructure.Data;

namespace SMCV.Infrastructure.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email) =>
        await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
}
