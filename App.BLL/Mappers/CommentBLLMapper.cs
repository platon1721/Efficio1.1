using Base.Contracts;

namespace App.BLL.Mappers;

public class CommentBLLMapper : IMapper<App.BLL.DTO.Comment, App.DAL.DTO.Comment>
{
    public App.BLL.DTO.Comment? Map(App.DAL.DTO.Comment? entity)
    {
        if (entity == null) return null;
        
        return new App.BLL.DTO.Comment
        {
            Id = entity.Id,
            Content = entity.Content,
            PostId = entity.PostId,
            Post = entity.Post != null ? new App.BLL.DTO.Post
            {
                Id = entity.Post.Id,
                Title = entity.Post.Title,
                Description = entity.Post.Description
            } : null,
            FeedbackId = entity.FeedbackId,
            Feedback = entity.Feedback != null ? new App.BLL.DTO.Feedback
            {
                Id = entity.Feedback.Id,
                Title = entity.Feedback.Title,
                Description = entity.Feedback.Description
            } : null
        };
    }

    public App.DAL.DTO.Comment? Map(App.BLL.DTO.Comment? entity)
    {
        if (entity == null) return null;
        
        return new App.DAL.DTO.Comment
        {
            Id = entity.Id,
            Content = entity.Content,
            PostId = entity.PostId,
            FeedbackId = entity.FeedbackId
        };
    }
}