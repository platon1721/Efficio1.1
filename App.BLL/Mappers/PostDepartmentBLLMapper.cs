using Base.Contracts;

namespace App.BLL.Mappers;

public class PostDepartmentBLLMapper : IMapper<App.BLL.DTO.PostDepartment, App.DAL.DTO.PostDepartment>
{
    public App.BLL.DTO.PostDepartment? Map(App.DAL.DTO.PostDepartment? entity)
    {
        if (entity == null) return null;
        
        return new App.BLL.DTO.PostDepartment
        {
            Id = entity.Id,
            PostId = entity.PostId,
            DepartmentId = entity.DepartmentId,
            Post = entity.Post != null ? new App.BLL.DTO.Post
            {
                Id = entity.Post.Id,
                Title = entity.Post.Title,
                Description = entity.Post.Description
            } : null,
            Department = entity.Department != null ? new App.BLL.DTO.Department
            {
                Id = entity.Department.Id,
                DepartmentName = entity.Department.DepartmentName,
                ManagerId = entity.Department.ManagerId
            } : null
        };
    }

    public App.DAL.DTO.PostDepartment? Map(App.BLL.DTO.PostDepartment? entity)
    {
        if (entity == null) return null;
        
        return new App.DAL.DTO.PostDepartment
        {
            Id = entity.Id,
            PostId = entity.PostId,
            DepartmentId = entity.DepartmentId
        };
    }
}