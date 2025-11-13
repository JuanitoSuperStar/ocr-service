# OCR Service

A .NET 9 web API service for optical character recognition (OCR) using Tesseract.

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Tesseract OCR](https://github.com/tesseract-ocr/tesseract) (for local development)
  - On Ubuntu/Debian: `sudo apt-get install tesseract-ocr tesseract-ocr-eng tesseract-ocr-spa`
  - On Windows: Download from [UB-Mannheim/tesseract](https://github.com/UB-Mannheim/tesseract/wiki)
  - On macOS: `brew install tesseract`

## Running Locally

1. Restore dependencies:
   ```bash
   dotnet restore
   ```

2. Run the application:
   ```bash
   dotnet run --project src/OcrService.Api
   ```

3. Access the API:
   - Swagger UI: http://localhost:5067/swagger
   - API endpoint: http://localhost:5067

## Running with Docker

1. Build the Docker image:
   ```bash
   docker build -t ocr-service .
   ```

2. Run the container:
   ```bash
   docker run -p 8080:8080 ocr-service
   ```

3. Access the API:
   - Swagger UI: http://localhost:8080/swagger
   - API endpoint: http://localhost:8080

## API Endpoints

- `POST /ocr/process` - Process an image for OCR
- `GET /swagger` - API documentation

## Project Structure

- `src/OcrService.Api/` - Web API project
- `src/OcrService.Application/` - Application logic
- `src/OcrService.Domain/` - Domain models and interfaces
- `src/OcrService.Infrastructure/` - Infrastructure implementations (Tesseract OCR)

## Dependencies

- ASP.NET Core 9.0
- Tesseract OCR (installed separately)
- Swashbuckle for Swagger UI