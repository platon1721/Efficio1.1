using Base.Contracts;

namespace App.DAL.EF.Mappers;

public class PostDepartmentUOWMapper : IMapper<App.DAL.DTO.PostDepartment, App.Domain.PostDepartment>
{
    public App.DAL.DTO.PostDepartment? Map(App.Domain.PostDepartment? entity)
    {
        if (entity == null) return null;
        
        return new App.DAL.DTO.PostDepartment
        {
            Id = entity.Id,
            PostId = entity.PostId,
            DepartmentId = entity.DepartmentId,
            Post = entity.Post != null ? new App.DAL.DTO.Post
            {
                Id = entity.Post.Id,
                Title = entity.Post.Title,
                Description = entity.Post.Description
            } : null,
            Department = entity.Department != null ? new App.DAL.DTO.Department
            {
                Id = entity.Department.Id,
                DepartmentName = entity.Department.DepartmentName,
                ManagerId = entity.Department.ManagerId
            } : null
        };
    }

    public App.Domain.PostDepartment? Map(App.DAL.DTO.PostDepartment? entity)
    {
        if (entity == null) return null;
        
        return new App.Domain.PostDepartment
        {
            Id = entity.Id,
            PostId = entity.PostId,
            DepartmentId = entity.DepartmentId
        };
    }
}