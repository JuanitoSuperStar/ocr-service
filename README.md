# OCR Service

A .NET 8 web API service for optical character recognition (OCR) using Tesseract.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/download/) (database)
- [Tesseract OCR](https://github.com/tesseract-ocr/tesseract) (for local development)
  - On Ubuntu/Debian: `sudo apt-get install tesseract-ocr tesseract-ocr-eng tesseract-ocr-spa`
  - On Windows: Download from [UB-Mannheim/tesseract](https://github.com/UB-Mannheim/tesseract/wiki)
  - On macOS: `brew install tesseract`

## Database Setup

1. Install PostgreSQL and create a database named `ocrdb`.
2. Update the connection string in `src/OcrService.Api/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Database=ocrdb;Username=your_username;Password=your_password"
   }
   ```
3. Run migrations:
   ```bash
   dotnet ef database update --project src/OcrService.Api --startup-project src/OcrService.Api
   ```

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

### Using Docker Compose (Recommended)

1. Run the application with PostgreSQL:
   ```bash
   docker-compose up --build
   ```

2. Access the API:
   - Swagger UI: http://localhost:8080/swagger
   - API endpoint: http://localhost:8080

### Manual Docker Setup

1. Start PostgreSQL:
   ```bash
   docker run -d --name postgres-db -e POSTGRES_DB=ocrdb -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=password -p 5432:5432 postgres:15
   ```

2. Build the application image:
   ```bash
   docker build -t ocr-service .
   ```

3. Run the application container:
   ```bash
   docker run -p 8080:8080 -e ConnectionStrings__DefaultConnection="Host=host.docker.internal;Database=ocrdb;Username=postgres;Password=password" ocr-service
   ```

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