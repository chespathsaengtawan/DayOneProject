using DayOneAPI.Interfaces;

namespace DayOneAPI.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _uploadsRoot;
    private readonly string _baseUrl;

    public LocalFileStorageService(IWebHostEnvironment env, IConfiguration config)
    {
        _uploadsRoot = Path.Combine(env.ContentRootPath, "uploads");
        _baseUrl = config["App:BaseUrl"]?.TrimEnd('/') ?? string.Empty;
    }

    public async Task<(string url, string storagePath)> UploadAsync(
        Stream fileStream, string fileName, string mimeType, string folder)
    {
        // Validate folder contains only GUIDs separated by slash (security)
        var parts = folder.Split('/');
        if (parts.Any(p => !Guid.TryParse(p, out _)))
            throw new InvalidOperationException("Invalid storage path");

        var storagePath = $"{folder}/{fileName}";
        var fullDir     = Path.Combine(_uploadsRoot, folder.Replace('/', Path.DirectorySeparatorChar));

        Directory.CreateDirectory(fullDir);

        var fullPath = Path.Combine(fullDir, fileName);

        using var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None);
        await fileStream.CopyToAsync(fs);

        var url = $"{_baseUrl}/uploads/{storagePath}";
        return (url, storagePath);
    }

    public Task DeleteAsync(string storagePath)
    {
        var safePath = storagePath.Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.Combine(_uploadsRoot, safePath);

        // Ensure path stays within uploads root (prevent path traversal)
        var resolved = Path.GetFullPath(fullPath);
        if (!resolved.StartsWith(Path.GetFullPath(_uploadsRoot), StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Invalid storage path");

        if (File.Exists(resolved))
            File.Delete(resolved);

        return Task.CompletedTask;
    }
}
