using Base.Contracts;

namespace App.DAL.EF.Mappers;

public class FeedbackTagUOWMapper : IMapper<App.DAL.DTO.FeedbackTag, App.Domain.FeedbackTag>
{
    public App.DAL.DTO.FeedbackTag? Map(App.Domain.FeedbackTag? entity)
    {
        if (entity == null) return null;
        
        return new App.DAL.DTO.FeedbackTag
        {
            Id = entity.Id,
            FeedbackId = entity.FeedbackId,
            TagId = entity.TagId,
            Feedback = entity.Feedback != null ? new App.DAL.DTO.Feedback
            {
                Id = entity.Feedback.Id,
                Title = entity.Feedback.Title,
                Description = entity.Feedback.Description
            } : null,
            Tag = entity.Tag != null ? new App.DAL.DTO.Tag
            {
                Id = entity.Tag.Id,
                Title = entity.Tag.Title,
                Description = entity.Tag.Description
            } : null
        };
    }

    public App.Domain.FeedbackTag? Map(App.DAL.DTO.FeedbackTag? entity)
    {
        if (entity == null) return null;
        
        return new App.Domain.FeedbackTag
        {
            Id = entity.Id,
            FeedbackId = entity.FeedbackId,
            TagId = entity.TagId
        };
    }
}