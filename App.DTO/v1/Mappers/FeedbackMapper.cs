using Base.Contracts;

namespace App.DTO.v1.Mappers;

public class FeedbackMapper : IMapper<App.DTO.v1.Feedback, App.BLL.DTO.Feedback>
{
    public App.DTO.v1.Feedback? Map(App.BLL.DTO.Feedback? entity)
    {
        if (entity == null) return null;
        
        return new App.DTO.v1.Feedback
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            DepartmentId = entity.DepartmentId,
            DepartmentName = entity.Department?.DepartmentName,
            TagTitles = entity.Tags?.Select(t => t.Title).ToList(),
            CommentCount = entity.Comments?.Count() ?? 0
        };
    }

    public App.BLL.DTO.Feedback? Map(App.DTO.v1.Feedback? entity)
    {
        if (entity == null) return null;
        
        return new App.BLL.DTO.Feedback
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            DepartmentId = entity.DepartmentId
        };
    }
    
    public App.BLL.DTO.Feedback Map(App.DTO.v1.CreateFeedback entity)
    {
        return new App.BLL.DTO.Feedback
        {
            Id = Guid.NewGuid(),
            Title = entity.Title,
            Description = entity.Description,
            DepartmentId = entity.DepartmentId
        };
    }
}