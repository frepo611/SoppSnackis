using SoppSnackis.DTOs;
using SoppSnackis.Models;
using System.Text;
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

    // --- Forbidden Words API Methods ---

    public async Task<List<ForbiddenWord>> GetForbiddenWordsAsync()
    {
        var client = _httpClientFactory.CreateClient("SoppSnackisAPI");
        var response = await client.GetAsync("admin/forbiddenwords");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var words = JsonSerializer.Deserialize<List<ForbiddenWord>>(json, options) ?? new List<ForbiddenWord>();
        return words;
    }

    public async Task DeleteForbiddenWordAsync(int id)
    {
        var client = _httpClientFactory.CreateClient("SoppSnackisAPI");
        var response = await client.DeleteAsync($"admin/forbiddenwords/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateForbiddenWordAsync(int id, string newWord)
    {
        var client = _httpClientFactory.CreateClient("SoppSnackisAPI");
        var content = new StringContent(JsonSerializer.Serialize(newWord), Encoding.UTF8, "application/json");
        var response = await client.PutAsync($"admin/forbiddenwords/{id}", content);
        response.EnsureSuccessStatusCode();
    }

    public async Task CreateForbiddenWordAsync(string word)
    {
        var client = _httpClientFactory.CreateClient("SoppSnackisAPI");
        var content = new StringContent(JsonSerializer.Serialize(word), Encoding.UTF8, "application/json");
        var response = await client.PostAsync("admin/forbiddenwords", content);
        response.EnsureSuccessStatusCode();
    }

    public async Task CreateForbiddenWordAsync(string word, Guid createdByUserId)
    {
        var client = _httpClientFactory.CreateClient("SoppSnackisAPI");
        var payload = new
        {
            Word = word,
            CreatedByUserId = createdByUserId
        };
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var response = await client.PostAsync("admin/forbiddenwords", content);
        response.EnsureSuccessStatusCode();
    }
}