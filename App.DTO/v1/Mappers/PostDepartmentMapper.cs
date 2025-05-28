using Base.Contracts;

namespace App.DTO.v1.Mappers;

public class PostDepartmentMapper : IMapper<App.DTO.v1.PostDepartment, App.BLL.DTO.PostDepartment>
{
    public App.DTO.v1.PostDepartment? Map(App.BLL.DTO.PostDepartment? entity)
    {
        if (entity == null) return null;
        
        return new App.DTO.v1.PostDepartment
        {
            Id = entity.Id,
            PostId = entity.PostId,
            DepartmentId = entity.DepartmentId,
            PostTitle = entity.Post?.Title,
            DepartmentName = entity.Department?.DepartmentName
        };
    }

    public App.BLL.DTO.PostDepartment? Map(App.DTO.v1.PostDepartment? entity)
    {
        if (entity == null) return null;
        
        return new App.BLL.DTO.PostDepartment
        {
            Id = entity.Id,
            PostId = entity.PostId,
            DepartmentId = entity.DepartmentId
        };
    }
}