namespace OcrService.Domain.Entities;

public class OcrResult
{
    public string Id { get; set; } = string.Empty;
    public JobStatus Status { get; set; }
    public string? ExtractedText { get; set; }
    public double? Confidence { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}