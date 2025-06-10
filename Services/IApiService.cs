using SoppSnackis.DTOs;
using SoppSnackis.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SoppSnackis.Services;

public interface IApiService
{
    Task<string> GetStatusAsync();
    Task<List<TopicDTO>> GetTopicsAsync();
    Task CreateTopicAsync(TopicDTO newTopic);
    Task DeleteTopicAsync(int id);

    // --- Forbidden Words API Methods ---
    Task<List<ForbiddenWord>> GetForbiddenWordsAsync();
    Task DeleteForbiddenWordAsync(int id);
    Task UpdateForbiddenWordAsync(int id, string newWord);
    Task CreateForbiddenWordAsync(string word, Guid createdByUserId);
}
