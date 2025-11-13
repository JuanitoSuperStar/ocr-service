using OcrService.Domain.Entities;

namespace OcrService.Domain.Interfaces;

public interface IOcrJobRepository
{
    Task<OcrJob?> GetByIdAsync(string id);
    Task AddAsync(OcrJob job);
    Task UpdateAsync(OcrJob job);
    Task<IEnumerable<OcrJob>> GetAllAsync();
}