using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Raceday.Api.Services;

public interface IBlobStorageService
{
    Task<string> UploadAsync(Stream stream, string fileName, string contentType);
    Task DeleteAsync(string fileUrl);
}

public class BlobStorageService : IBlobStorageService
{
    private readonly BlobContainerClient _container;
    private readonly ILogger<BlobStorageService> _logger;

    public BlobStorageService(IConfiguration config, ILogger<BlobStorageService> logger)
    {
        _logger = logger;
        var conn = config.GetConnectionString("AzureBlobStorage")
                   ?? throw new InvalidOperationException("AzureBlobStorage connection string missing.");
        var containerName = config["AzureBlob:ContainerName"] ?? "raceday-images";
        _container = new BlobContainerClient(conn, containerName);
        _container.CreateIfNotExists(PublicAccessType.Blob);
    }

    public async Task<string> UploadAsync(Stream stream, string fileName, string contentType)
    {
        var ext = Path.GetExtension(fileName);
        var blobName = $"{Guid.NewGuid()}{ext}";
        var blob = _container.GetBlobClient(blobName);

        await blob.UploadAsync(stream, new BlobHttpHeaders { ContentType = contentType });
        _logger.LogInformation("Uploaded {BlobName} ({ContentType})", blobName, contentType);
        return blob.Uri.ToString();
    }

    public async Task DeleteAsync(string fileUrl)
    {
        if (string.IsNullOrWhiteSpace(fileUrl)) return;
        try
        {
            var uri = new Uri(fileUrl);
            var name = Path.GetFileName(uri.LocalPath);
            await _container.DeleteBlobIfExistsAsync(name);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not delete blob {Url}", fileUrl);
        }
    }
}
