using Base.Contracts;

namespace App.DTO.v1.Mappers;

public class DepartmentMapper: IMapper<App.DTO.v1.Department, App.BLL.DTO.Department>
{
    public App.DTO.v1.Department? Map(BLL.DTO.Department? entity)
    {
        if (entity == null) return null;
        var res = new App.DTO.v1.Department()
        {
            Id = entity.Id,
            DepartmentName = entity.DepartmentName
        };
        if (entity.ManagerId.HasValue) res.ManagerId = entity.ManagerId.Value;
        return res;
    }

    public BLL.DTO.Department? Map(App.DTO.v1.Department? entity)
    {
        if (entity == null) return null;
        var res = new BLL.DTO.Department()
        {
            Id = entity.Id,
            DepartmentName = entity.DepartmentName
        };
        if (entity.ManagerId.HasValue) res.ManagerId = entity.ManagerId.Value;
        return res;
    }
}