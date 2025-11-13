using OcrService.Domain.Interfaces;
using OcrService.Infrastructure;
using OcrService.Infrastructure.Repositories;
using OcrService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore;
using System.Threading;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<OcrDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("OcrService.Api")));

// Repositories
builder.Services.AddScoped<IOcrJobRepository, OcrJobRepository>();

// Services
builder.Services.AddScoped<IOcrProcessingService, OcrProcessingService>();

var app = builder.Build();

// Run migrations on startup with retry logic
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<OcrDbContext>();
    var maxRetries = 10;
    var retryDelay = TimeSpan.FromSeconds(5);

    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            dbContext.Database.Migrate();
            break; // Success, exit loop
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Migration attempt {i + 1} failed: {ex.Message}");
            if (i == maxRetries - 1)
            {
                throw; // Re-throw on last attempt
            }
            Thread.Sleep(retryDelay);
        }
    }
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
