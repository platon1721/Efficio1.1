using Base.Contracts;

namespace App.DAL.EF.Mappers;

public class PostUOWMapper : IMapper<App.DAL.DTO.Post, App.Domain.Post>
{
    public App.DAL.DTO.Post? Map(App.Domain.Post? entity)
    {
        if (entity == null) return null;
        
        return new App.DAL.DTO.Post
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            PostTags = entity.PostTags?.Select(pt => new App.DAL.DTO.PostTag
            {
                Id = pt.Id,
                PostId = pt.PostId,
                TagId = pt.TagId,
                Tag = pt.Tag != null ? new App.DAL.DTO.Tag
                {
                    Id = pt.Tag.Id,
                    Title = pt.Tag.Title,
                    Description = pt.Tag.Description
                } : null
            }).ToList(),
            PostDepartments = entity.PostDepartments?.Select(pd => new App.DAL.DTO.PostDepartment
            {
                Id = pd.Id,
                PostId = pd.PostId,
                DepartmentId = pd.DepartmentId,
                Department = pd.Department != null ? new App.DAL.DTO.Department
                {
                    Id = pd.Department.Id,
                    DepartmentName = pd.Department.DepartmentName,
                    ManagerId = pd.Department.ManagerId
                } : null
            }).ToList()
        };
    }

    public App.Domain.Post? Map(App.DAL.DTO.Post? entity)
    {
        if (entity == null) return null;
        
        return new App.Domain.Post
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description
            // PostTags and PostDepartments will be handled separately
        };
    }
}