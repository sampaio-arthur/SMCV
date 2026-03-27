using Microsoft.EntityFrameworkCore;
using DesenvWebApi.Data;
using DesenvWebApi.Entities;

namespace DesenvWebApi.Repositories;

public class ExampleRepository : IExampleRepository
{
    private readonly AppDbContext _context;

    public ExampleRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Example>> GetAllAsync()
    {
        return await _context.Examples.ToListAsync();
    }

    public async Task<Example?> GetByIdAsync(int id)
    {
        return await _context.Examples.FindAsync(id);
    }

    public async Task<Example> CreateAsync(Example entity)
    {
        _context.Examples.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Example> UpdateAsync(Example entity)
    {
        _context.Examples.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.Examples.FindAsync(id);
        if (entity == null) return false;

        _context.Examples.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
