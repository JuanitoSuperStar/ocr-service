using OcrService.Domain.Entities;

namespace OcrService.Domain.Interfaces;

public interface IOcrProcessingService
{
    Task<string> StartOcrJob(byte[] fileData, string fileName, string contentType, string language = "eng");
    Task<OcrResult?> GetJobResult(string id);
}