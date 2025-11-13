using Microsoft.EntityFrameworkCore;
using OcrService.Domain.Entities;
using OcrService.Domain.Interfaces;

namespace OcrService.Infrastructure.Repositories;

public class OcrJobRepository : IOcrJobRepository
{
    private readonly OcrDbContext _context;

    public OcrJobRepository(OcrDbContext context)
    {
        _context = context;
    }

    public async Task<OcrJob?> GetByIdAsync(string id)
    {
        return await _context.OcrJobs.FindAsync(id);
    }

    public async Task AddAsync(OcrJob job)
    {
        await _context.OcrJobs.AddAsync(job);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(OcrJob job)
    {
        _context.OcrJobs.Update(job);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<OcrJob>> GetAllAsync()
    {
        return await _context.OcrJobs.ToListAsync();
    }
}