using SoppSnackis.DTOs;
using System.Threading.Tasks;

namespace SoppSnackis.Services;

public interface IApiService
{
    Task<string> GetStatusAsync();
    // Add more methods as needed, e.g., Task<List<Post>> GetPostsAsync();
    Task<List<TopicDTO>> GetTopicsAsync();
    Task CreateTopicAsync(TopicDTO newTopic);
    Task DeleteTopicAsync(int id);
}
