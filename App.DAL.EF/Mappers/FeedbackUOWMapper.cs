using Base.Contracts;

namespace App.DAL.EF.Mappers;

public class FeedbackUOWMapper : IMapper<App.DAL.DTO.Feedback, App.Domain.Feedback>
{
    public App.DAL.DTO.Feedback? Map(App.Domain.Feedback? entity)
    {
        if (entity == null) return null;
        
        return new App.DAL.DTO.Feedback
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            DepartmentId = entity.DepartmentId,
            Department = entity.Department != null ? new App.DAL.DTO.Department
            {
                Id = entity.Department.Id,
                DepartmentName = entity.Department.DepartmentName,
                ManagerId = entity.Department.ManagerId
            } : null,
            FeedbackTags = entity.FeedbackTags?.Select(ft => new App.DAL.DTO.FeedbackTag
            {
                Id = ft.Id,
                FeedbackId = ft.FeedbackId,
                TagId = ft.TagId,
                Tag = ft.Tag != null ? new App.DAL.DTO.Tag
                {
                    Id = ft.Tag.Id,
                    Title = ft.Tag.Title,
                    Description = ft.Tag.Description
                } : null
            }).ToList(),
            Comments = entity.Comments?.Select(c => new App.DAL.DTO.Comment
            {
                Id = c.Id,
                Content = c.Content,
                PostId = c.PostId,
                FeedbackId = c.FeedbackId
            }).ToList()
        };
    }

    public App.Domain.Feedback? Map(App.DAL.DTO.Feedback? entity)
    {
        if (entity == null) return null;
        
        return new App.Domain.Feedback
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            DepartmentId = entity.DepartmentId
            // FeedbackTags and Comments will be handled separately
        };
    }
}