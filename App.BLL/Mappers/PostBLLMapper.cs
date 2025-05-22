using Base.Contracts;

namespace App.BLL.Mappers;

public class PostBLLMapper : IMapper<App.BLL.DTO.Post, App.DAL.DTO.Post>
{
    public App.BLL.DTO.Post? Map(App.DAL.DTO.Post? entity)
    {
        if (entity == null) return null;
        
        return new App.BLL.DTO.Post
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            PostTags = entity.PostTags?.Select(pt => new App.BLL.DTO.PostTag
            {
                Id = pt.Id,
                PostId = pt.PostId,
                TagId = pt.TagId,
                Tag = pt.Tag != null ? new App.BLL.DTO.Tag
                {
                    Id = pt.Tag.Id,
                    Title = pt.Tag.Title,
                    Description = pt.Tag.Description
                } : null
            }).ToList(),
            PostDepartments = entity.PostDepartments?.Select(pd => new App.BLL.DTO.PostDepartment
            {
                Id = pd.Id,
                PostId = pd.PostId,
                DepartmentId = pd.DepartmentId,
                Department = pd.Department != null ? new App.BLL.DTO.Department
                {
                    Id = pd.Department.Id,
                    DepartmentName = pd.Department.DepartmentName,
                    ManagerId = pd.Department.ManagerId
                } : null
            }).ToList()
        };
    }

    public App.DAL.DTO.Post? Map(App.BLL.DTO.Post? entity)
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
                TagId = pt.TagId
            }).ToList(),
            PostDepartments = entity.PostDepartments?.Select(pd => new App.DAL.DTO.PostDepartment
            {
                Id = pd.Id,
                PostId = pd.PostId,
                DepartmentId = pd.DepartmentId
            }).ToList()
        };
    }
}