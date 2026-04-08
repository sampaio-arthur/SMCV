namespace SMCV.Application.Interfaces;

using SMCV.Domain.Entities;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
}
