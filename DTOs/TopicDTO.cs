namespace SoppSnackis.DTOs;

public class TopicDTO
{
       public int Id { get; set; }

       public string Name { get; set; }
    /// <summary>
    /// Gets or sets the unique identifier of the user who created the topic.
    /// </summary>
    public Guid CreatedByUserId { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the topic was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

}

