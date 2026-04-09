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

    public async Task<IEnumerable<User>> GetAllPagedAsync(int pageNumber, int pageSize) =>
        await _context.Users
            .OrderBy(u => u.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
}
