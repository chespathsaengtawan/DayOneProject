namespace DayOneAPI.Interfaces;

public interface IFileStorageService
{
    Task<(string url, string storagePath)> UploadAsync(
        Stream fileStream, string fileName, string mimeType, string folder);

    Task DeleteAsync(string storagePath);
}
