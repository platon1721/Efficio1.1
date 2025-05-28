using Base.Contracts;

namespace App.DTO.v1.Mappers;

public class CommentMapper : IMapper<App.DTO.v1.Comment, App.BLL.DTO.Comment>
{
    public App.DTO.v1.Comment? Map(App.BLL.DTO.Comment? entity)
    {
        if (entity == null) return null;
        
        return new App.DTO.v1.Comment
        {
            Id = entity.Id,
            Content = entity.Content,
            PostId = entity.PostId,
            PostTitle = entity.Post?.Title,
            FeedbackId = entity.FeedbackId,
            FeedbackTitle = entity.Feedback?.Title,
            CreatedAt = DateTime.UtcNow, // Would need to get from BaseEntity
            AuthorName = "Current User" // Would need to get from context
        };
    }

    public App.BLL.DTO.Comment? Map(App.DTO.v1.Comment? entity)
    {
        if (entity == null) return null;
        
        return new App.BLL.DTO.Comment
        {
            Id = entity.Id,
            Content = entity.Content,
            PostId = entity.PostId,
            FeedbackId = entity.FeedbackId
        };
    }
    
    public App.BLL.DTO.Comment Map(App.DTO.v1.CreateComment entity)
    {
        return new App.BLL.DTO.Comment
        {
            Id = Guid.NewGuid(),
            Content = entity.Content,
            PostId = entity.PostId,
            FeedbackId = entity.FeedbackId
        };
    }
}