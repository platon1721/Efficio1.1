using Base.Contracts;

namespace App.DTO.v1.Mappers;

public class DepartmentPersonMapper : IMapper<App.DTO.v1.DepartmentPerson, App.BLL.DTO.DepartmentPerson>
{
    public App.DTO.v1.DepartmentPerson? Map(App.BLL.DTO.DepartmentPerson? entity)
    {
        if (entity == null) return null;
        
        return new App.DTO.v1.DepartmentPerson
        {
            Id = entity.Id,
            DepartmentId = entity.DepartmentId,
            PersonId = entity.PersonId,
            Position = entity.Position
        };
    }

    public App.BLL.DTO.DepartmentPerson? Map(App.DTO.v1.DepartmentPerson? entity)
    {
        if (entity == null) return null;
        
        return new App.BLL.DTO.DepartmentPerson
        {
            Id = entity.Id,
            DepartmentId = entity.DepartmentId,
            PersonId = entity.PersonId,
            Position = entity.Position
        };
    }
    
    public App.BLL.DTO.DepartmentPerson Map(App.DTO.v1.CreateDepartmentPerson entity)
    {
        return new App.BLL.DTO.DepartmentPerson
        {
            Id = Guid.NewGuid(),
            DepartmentId = entity.DepartmentId,
            PersonId = entity.PersonId,
            Position = entity.Position
        };
    }
}