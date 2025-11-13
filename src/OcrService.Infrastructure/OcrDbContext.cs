using Microsoft.EntityFrameworkCore;
using OcrService.Domain.Entities;

namespace OcrService.Infrastructure;

public class OcrDbContext : DbContext
{
    public OcrDbContext(DbContextOptions<OcrDbContext> options) : base(options) { }

    public DbSet<OcrJob> OcrJobs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OcrJob>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Status).HasConversion<string>();
            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.ContentType).HasMaxLength(100);
            entity.Property(e => e.Language).HasMaxLength(10);
            entity.Property(e => e.ExtractedText).HasColumnType("text");
            entity.Property(e => e.ErrorMessage).HasColumnType("text");
        });
    }
}