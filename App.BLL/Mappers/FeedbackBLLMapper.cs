using Base.Contracts;

namespace App.BLL.Mappers;

public class FeedbackBLLMapper : IMapper<App.BLL.DTO.Feedback, App.DAL.DTO.Feedback>
{
    public App.BLL.DTO.Feedback? Map(App.DAL.DTO.Feedback? entity)
    {
        if (entity == null) return null;
        
        return new App.BLL.DTO.Feedback
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            DepartmentId = entity.DepartmentId,
            Department = entity.Department != null ? new App.BLL.DTO.Department
            {
                Id = entity.Department.Id,
                DepartmentName = entity.Department.DepartmentName,
                ManagerId = entity.Department.ManagerId
            } : null,
            FeedbackTags = entity.FeedbackTags?.Select(ft => new App.BLL.DTO.FeedbackTag
            {
                Id = ft.Id,
                FeedbackId = ft.FeedbackId,
                TagId = ft.TagId,
                Tag = ft.Tag != null ? new App.BLL.DTO.Tag
                {
                    Id = ft.Tag.Id,
                    Title = ft.Tag.Title,
                    Description = ft.Tag.Description
                } : null
            }).ToList(),
            Comments = entity.Comments?.Select(c => new App.BLL.DTO.Comment
            {
                Id = c.Id,
                Content = c.Content,
                PostId = c.PostId,
                FeedbackId = c.FeedbackId
            }).ToList()
        };
    }

    public App.DAL.DTO.Feedback? Map(App.BLL.DTO.Feedback? entity)
    {
        if (entity == null) return null;
        
        return new App.DAL.DTO.Feedback
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            DepartmentId = entity.DepartmentId,
            FeedbackTags = entity.FeedbackTags?.Select(ft => new App.DAL.DTO.FeedbackTag
            {
                Id = ft.Id,
                FeedbackId = ft.FeedbackId,
                TagId = ft.TagId
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
}