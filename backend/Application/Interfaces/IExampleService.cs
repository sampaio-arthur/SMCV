using SMCV.Application.DTOs;

namespace SMCV.Application.Interfaces;

public interface IExampleService
{
    Task<IEnumerable<ExampleResponseDto>> GetAllAsync();
    Task<ExampleResponseDto?> GetByIdAsync(int id);
    Task<ExampleResponseDto> CreateAsync(ExampleRequestDto dto);
    Task<ExampleResponseDto?> UpdateAsync(int id, ExampleRequestDto dto);
    Task<bool> DeleteAsync(int id);
}
