# Use the official .NET 9 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /

# Copy the solution file and restore dependencies
COPY ["OcrService.sln", "./"]
COPY ["src/OcrService.Domain/OcrService.Domain.csproj", "src/OcrService.Domain/"]
COPY ["src/OcrService.Application/OcrService.Application.csproj", "src/OcrService.Application/"]
COPY ["src/OcrService.Infrastructure/OcrService.Infrastructure.csproj", "src/OcrService.Infrastructure/"]
COPY ["src/OcrService.Api/OcrService.Api.csproj", "src/OcrService.Api/"]
RUN dotnet restore "OcrService.sln"

# Copy the rest of the source code
COPY src/. src/

# Build the app
RUN dotnet build "src/OcrService.Api/OcrService.Api.csproj" -c Release -o /app/build

# Publish the app
FROM build AS publish
RUN dotnet publish "src/OcrService.Api/OcrService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use the official .NET 9 runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Install system dependencies for Tesseract
RUN apt-get update && apt-get install -y \
    tesseract-ocr \
    tesseract-ocr-eng \
    tesseract-ocr-spa \
    && rm -rf /var/lib/apt/lists/*

# Copy the published app
COPY --from=publish /app/publish .

# Expose the port
EXPOSE 8080

# Set the ASP.NET Core URLs
ENV ASPNETCORE_URLS=http://+:8080

# Set the entry point
ENTRYPOINT ["dotnet", "OcrService.Api.dll"]