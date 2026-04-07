using SMCV.Domain.Entities;

namespace SMCV.Application.Interfaces;

public interface IExampleRepository
{
    Task<IEnumerable<Example>> GetAllAsync();
    Task<Example?> GetByIdAsync(int id);
    Task<Example> CreateAsync(Example entity);
    Task<Example> UpdateAsync(Example entity);
    Task<bool> DeleteAsync(int id);
}
