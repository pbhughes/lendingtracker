using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

public class BlobStorageService : IBlobStorageService
{
    private readonly string _storageConnectionString;
    private readonly string _containerName;
    private ILogger _logger;
    private IConfiguration _configuration;
    public BlobStorageService(ILogger<BlobStorageService> logger, IConfiguration config)
    {
        _logger = logger;
        _configuration = config;
        _storageConnectionString = _configuration["BlobStorage:ConnectionString"];
        _containerName = _configuration["BlobStorage:ContainerName"];
    }

    public async Task<string> UploadImageAndGetSasAsync(Stream fileStream, string fileName, string contentType, TimeSpan sasDuration)
    {
        var blobServiceClient = new BlobServiceClient(_storageConnectionString);
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = blobContainerClient.GetBlobClient(fileName);

        // Upload the image
        await blobClient.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = contentType });

        // Generate SAS token
        string sasToken = GenerateSasToken(blobClient, sasDuration);

        // Return the full URL with SAS token
        return blobClient.Uri + sasToken;
    }

    private string GenerateSasToken(BlobClient blobClient, TimeSpan duration)
    {
        // Get storage account key
        var blobSasBuilder = new BlobSasBuilder
        {
            BlobContainerName = _containerName,
            BlobName = blobClient.Name,
            Resource = "b", // "b" for blob
            StartsOn = DateTime.Now.AddHours(-3),
            ExpiresOn = DateTime.UtcNow.Add(duration)
        };

        // Define SAS permissions (read access)
        blobSasBuilder.SetPermissions(BlobSasPermissions.Read);

        // Generate SAS token using storage account key
        var blobUriBuilder = new BlobUriBuilder(blobClient.Uri)
        {
            Sas = blobSasBuilder.ToSasQueryParameters(new Azure.Storage.StorageSharedKeyCredential(
               _configuration["BlobStorage:Name"], _configuration["BlobStorage:Key"]))
        };

        return "?" + blobUriBuilder.Sas;
    }
}
