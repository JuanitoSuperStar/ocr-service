using Microsoft.AspNetCore.Mvc;
using OcrService.Domain.Interfaces;

namespace OcrService.Api.Controllers;

[ApiController]
[Route("api/ocr")]
public class OcrController : ControllerBase
{
    private readonly IOcrProcessingService _ocrService;

    public OcrController(IOcrProcessingService ocrService)
    {
        _ocrService = ocrService;
    }

    [HttpPost("process")]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> ProcessDocument(IFormFile file, [FromQuery] string? lang)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        var allowedTypes = new[] { "image/jpeg", "image/png", "image/bmp", "image/tiff", "application/pdf" };
        if (!allowedTypes.Contains(file.ContentType.ToLower()))
        {
            return BadRequest("Unsupported file type. Only JPEG, PNG, BMP, TIFF images and PDF are supported.");
        }

        if (file.Length > 10 * 1024 * 1024) // 10MB limit
        {
            return BadRequest("File too large. Maximum size is 10MB.");
        }

        var language = lang ?? "eng"; // Default to English
        using var stream = file.OpenReadStream();
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        var fileData = memoryStream.ToArray();
        var jobId = await _ocrService.StartOcrJob(fileData, file.FileName, file.ContentType, language);

        // Get the job details to return more complete response
        var job = await _ocrService.GetJobResult(jobId);
        if (job != null)
        {
            return Ok(new
            {
                JobId = job.Id,
                Status = job.Status.ToString(),
                FileName = job.FileName,
                ContentType = job.ContentType,
                Language = job.Language,
                CreatedAt = job.CreatedAt,
                FileSize = file.Length
            });
        }

        return Ok(new { JobId = jobId });
    }

    [HttpGet("status/{id}")]
    public async Task<IActionResult> GetStatus(string id)
    {
        var result = await _ocrService.GetJobResult(id);
        if (result == null)
        {
            return NotFound("Job not found.");
        }
        return Ok(result);
    }
}