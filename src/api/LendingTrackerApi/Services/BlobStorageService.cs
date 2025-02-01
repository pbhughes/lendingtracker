using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;
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

        string fullUrl = $"{blobClient.Uri.ToString()}?{_configuration["BlobStorage:SasToken"]}";

        // Return the full URL with SAS token
        return fullUrl;
    }


    
}
