using Base.Contracts;

namespace App.DAL.EF.Mappers;

public class CommentUOWMapper : IMapper<App.DAL.DTO.Comment, App.Domain.Comment>
{
    public App.DAL.DTO.Comment? Map(App.Domain.Comment? entity)
    {
        if (entity == null) return null;
        
        return new App.DAL.DTO.Comment
        {
            Id = entity.Id,
            Content = entity.Content,
            PostId = entity.PostId,
            Post = entity.Post != null ? new App.DAL.DTO.Post
            {
                Id = entity.Post.Id,
                Title = entity.Post.Title,
                Description = entity.Post.Description
            } : null,
            FeedbackId = entity.FeedbackId,
            Feedback = entity.Feedback != null ? new App.DAL.DTO.Feedback
            {
                Id = entity.Feedback.Id,
                Title = entity.Feedback.Title,
                Description = entity.Feedback.Description
            } : null
        };
    }

    public App.Domain.Comment? Map(App.DAL.DTO.Comment? entity)
    {
        if (entity == null) return null;
        
        return new App.Domain.Comment
        {
            Id = entity.Id,
            Content = entity.Content,
            PostId = entity.PostId,
            FeedbackId = entity.FeedbackId
        };
    }
}