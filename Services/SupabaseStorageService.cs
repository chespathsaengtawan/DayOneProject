using System.Net.Http.Headers;
using DayOneAPI.Interfaces;

namespace DayOneAPI.Services;

public class SupabaseStorageService : IFileStorageService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _supabaseUrl;
    private readonly string _bucketName;

    public SupabaseStorageService(IHttpClientFactory httpClientFactory, IConfiguration config)
    {
        _httpClientFactory = httpClientFactory;
        _supabaseUrl = config["Supabase:Url"]!;
        _bucketName  = config["Supabase:StorageBucket"] ?? "event-images";
    }

    public async Task<(string url, string storagePath)> UploadAsync(
        Stream fileStream, string fileName, string mimeType, string folder)
    {
        var storagePath = $"{folder}/{fileName}";
        var client = _httpClientFactory.CreateClient("supabase");

        var content = new StreamContent(fileStream);
        content.Headers.ContentType = new MediaTypeHeaderValue(mimeType);

        var response = await client.PostAsync(
            $"{_supabaseUrl}/storage/v1/object/{_bucketName}/{storagePath}",
            content);
        response.EnsureSuccessStatusCode();

        var url = $"{_supabaseUrl}/storage/v1/object/public/{_bucketName}/{storagePath}";
        return (url, storagePath);
    }

    public async Task DeleteAsync(string storagePath)
    {
        var client = _httpClientFactory.CreateClient("supabase");

        var request = new HttpRequestMessage(HttpMethod.Delete,
            $"{_supabaseUrl}/storage/v1/object/{_bucketName}");
        request.Content = JsonContent.Create(new { prefixes = new[] { storagePath } });

        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
}
