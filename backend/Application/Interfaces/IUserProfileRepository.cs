namespace SMCV.Application.Interfaces;

using SMCV.Domain.Entities;

public interface IUserProfileRepository : IRepository<UserProfile>
{
    Task<UserProfile?> GetByUserIdAsync(Guid userId);
}
