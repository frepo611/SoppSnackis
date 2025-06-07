using SoppSnackis.DTOs;
using System.Text.Json;

namespace SoppSnackis.Services;

public class ApiService : IApiService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task CreateTopicAsync(TopicDTO newTopic)
    {
        var client = _httpClientFactory.CreateClient("SoppSnackisAPI");
        var json = JsonSerializer.Serialize(newTopic);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await client.PostAsync("admin/topics", content);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteTopicAsync(int id)
    {
        var client = _httpClientFactory.CreateClient("SoppSnackisAPI");
        var response = await client.DeleteAsync($"admin/topics/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<string> GetStatusAsync()
    {
        var client = _httpClientFactory.CreateClient("SoppSnackisAPI");
        var response = await client.GetAsync("status");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<List<TopicDTO>> GetTopicsAsync()
    {
        var client = _httpClientFactory.CreateClient("SoppSnackisAPI");
        var response = await client.GetAsync("admin/topics");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var topics = JsonSerializer.Deserialize<List<TopicDTO>>(json, options) ?? new List<TopicDTO>();
        return topics;
    }
}