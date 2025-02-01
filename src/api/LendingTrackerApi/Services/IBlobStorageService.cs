
public interface IBlobStorageService
{
    Task<string> UploadImageAndGetSasAsync(Stream fileStream, string fileName, string contentType, TimeSpan sasDuration);
}