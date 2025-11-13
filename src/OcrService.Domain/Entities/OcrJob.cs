namespace OcrService.Domain.Entities;

public enum JobStatus
{
    Pending,
    Processing,
    Completed,
    Failed
}

public class OcrJob
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public JobStatus Status { get; set; } = JobStatus.Pending;
    public string? FileName { get; set; }
    public string? ContentType { get; set; }
    public string Language { get; set; } = "eng";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public string? ExtractedText { get; set; }
    public double? Confidence { get; set; }
    public string? ErrorMessage { get; set; }
}