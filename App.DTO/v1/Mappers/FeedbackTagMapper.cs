using Base.Contracts;

namespace App.DTO.v1.Mappers;

public class FeedbackTagMapper : IMapper<App.DTO.v1.FeedbackTag, App.BLL.DTO.FeedbackTag>
{
    public App.DTO.v1.FeedbackTag? Map(App.BLL.DTO.FeedbackTag? entity)
    {
        if (entity == null) return null;
        
        return new App.DTO.v1.FeedbackTag
        {
            Id = entity.Id,
            FeedbackId = entity.FeedbackId,
            TagId = entity.TagId,
            FeedbackTitle = entity.Feedback?.Title,
            TagTitle = entity.Tag?.Title
        };
    }

    public App.BLL.DTO.FeedbackTag? Map(App.DTO.v1.FeedbackTag? entity)
    {
        if (entity == null) return null;
        
        return new App.BLL.DTO.FeedbackTag
        {
            Id = entity.Id,
            FeedbackId = entity.FeedbackId,
            TagId = entity.TagId
        };
    }
}