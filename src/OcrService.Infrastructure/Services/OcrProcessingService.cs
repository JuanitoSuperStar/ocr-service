using OcrService.Domain.Entities;
using OcrService.Domain.Interfaces;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace OcrService.Infrastructure.Services;

public class OcrProcessingService : IOcrProcessingService
{
    private readonly ConcurrentDictionary<string, OcrJob> _jobs = new();

    public string StartOcrJob(byte[] fileData, string fileName, string contentType, string language = "eng")
    {
        var job = new OcrJob
        {
            FileName = fileName,
            ContentType = contentType,
            Language = language
        };

        _jobs[job.Id] = job;

        // Start processing asynchronously
        Task.Run(() => ProcessOcrAsync(job, fileData, contentType));

        return job.Id;
    }

    public OcrResult? GetJobResult(string id)
    {
        if (!_jobs.TryGetValue(id, out var job))
            return null;

        return new OcrResult
        {
            Id = job.Id,
            Status = job.Status,
            ExtractedText = job.ExtractedText,
            Confidence = job.Confidence,
            ErrorMessage = job.ErrorMessage,
            CreatedAt = job.CreatedAt,
            CompletedAt = job.CompletedAt
        };
    }

    private async Task ProcessOcrAsync(OcrJob job, byte[] fileData, string contentType)
    {
        try
        {
            job.Status = JobStatus.Processing;

            string extractedText;
            double confidence;

            if (contentType.StartsWith("image/"))
            {
                (extractedText, confidence) = await ProcessImageAsync(fileData, job.Language);
            }
            else if (contentType == "application/pdf")
            {
                (extractedText, confidence) = await ProcessPdfAsync(fileData);
            }
            else
            {
                throw new NotSupportedException("Unsupported file type. Only images and PDFs are supported.");
            }

            job.ExtractedText = extractedText;
            job.Confidence = confidence;
            job.Status = JobStatus.Completed;
            job.CompletedAt = DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            job.Status = JobStatus.Failed;
            job.ErrorMessage = ex.Message;
            job.CompletedAt = DateTime.UtcNow;
        }
    }

    private async Task<(string text, double confidence)> ProcessImageAsync(byte[] imageData, string language)
    {
        try
        {
            var tempImagePath = Path.GetTempFileName() + ".png";
            var tempOutputPath = Path.GetTempFileName();

            await File.WriteAllBytesAsync(tempImagePath, imageData);

            var process = Process.Start(new ProcessStartInfo
            {
                FileName = "tesseract",
                Arguments = $"{tempImagePath} {tempOutputPath} -l {language} --oem 1 --psm 3",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            });

            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                var error = process.StandardError.ReadToEnd();
                throw new Exception($"Tesseract error: {error}");
            }

            var textFile = tempOutputPath + ".txt";
            if (!File.Exists(textFile))
            {
                throw new Exception("Tesseract did not produce output file");
            }

            var extractedText = File.ReadAllText(textFile).Trim();

            // For confidence, tesseract command line doesn't give mean confidence easily
            // Assume 90% for successful extraction
            double confidence = string.IsNullOrEmpty(extractedText) ? 0 : 90.0;

            // Cleanup
            File.Delete(tempImagePath);
            File.Delete(textFile);

            return (extractedText, confidence);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error processing image: {ex.Message}", ex);
        }
    }

    private async Task<(string text, double confidence)> ProcessPdfAsync(byte[] pdfData)
    {
        try
        {
            using var stream = new MemoryStream(pdfData);
            using var pdfReader = new iText.Kernel.Pdf.PdfReader(stream);
            using var pdfDoc = new iText.Kernel.Pdf.PdfDocument(pdfReader);

            var text = new System.Text.StringBuilder();
            for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
            {
                var page = pdfDoc.GetPage(i);
                var strategy = new iText.Kernel.Pdf.Canvas.Parser.Listener.LocationTextExtractionStrategy();
                var pageText = iText.Kernel.Pdf.Canvas.Parser.PdfTextExtractor.GetTextFromPage(page, strategy);
                text.Append(pageText);
            }

            // For PDF text extraction, assume high confidence if text is found
            var extractedText = text.ToString().Trim();
            double confidence = string.IsNullOrEmpty(extractedText) ? 0 : 95.0; // Assume 95% for extracted text

            return (extractedText, confidence);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error processing PDF: {ex.Message}", ex);
        }
    }
}