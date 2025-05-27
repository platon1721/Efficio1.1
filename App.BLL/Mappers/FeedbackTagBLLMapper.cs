using Base.Contracts;

namespace App.BLL.Mappers;

public class FeedbackTagBLLMapper : IMapper<App.BLL.DTO.FeedbackTag, App.DAL.DTO.FeedbackTag>
{
    public App.BLL.DTO.FeedbackTag? Map(App.DAL.DTO.FeedbackTag? entity)
    {
        if (entity == null) return null;
        
        return new App.BLL.DTO.FeedbackTag
        {
            Id = entity.Id,
            FeedbackId = entity.FeedbackId,
            TagId = entity.TagId,
            Feedback = entity.Feedback != null ? new App.BLL.DTO.Feedback
            {
                Id = entity.Feedback.Id,
                Title = entity.Feedback.Title,
                Description = entity.Feedback.Description
            } : null,
            Tag = entity.Tag != null ? new App.BLL.DTO.Tag
            {
                Id = entity.Tag.Id,
                Title = entity.Tag.Title,
                Description = entity.Tag.Description
            } : null
        };
    }

    public App.DAL.DTO.FeedbackTag? Map(App.BLL.DTO.FeedbackTag? entity)
    {
        if (entity == null) return null;
        
        return new App.DAL.DTO.FeedbackTag
        {
            Id = entity.Id,
            FeedbackId = entity.FeedbackId,
            TagId = entity.TagId
        };
    }
}