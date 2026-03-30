using SMCV.DTOs;

namespace SMCV.Services;

public interface IExampleService
{
    Task<IEnumerable<ExampleResponseDto>> GetAllAsync();
    Task<ExampleResponseDto?> GetByIdAsync(int id);
    Task<ExampleResponseDto> CreateAsync(ExampleRequestDto dto);
    Task<ExampleResponseDto?> UpdateAsync(int id, ExampleRequestDto dto);
    Task<bool> DeleteAsync(int id);
}
