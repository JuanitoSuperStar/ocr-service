using OcrService.Domain.Entities;

namespace OcrService.Domain.Interfaces;

public interface IOcrProcessingService
{
    string StartOcrJob(byte[] fileData, string fileName, string contentType, string language = "eng");
    OcrResult? GetJobResult(string id);
}