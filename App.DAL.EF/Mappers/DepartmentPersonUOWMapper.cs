// DepartmentPersonUOWMapper.cs
using Base.Contracts;

namespace App.DAL.EF.Mappers;

public class DepartmentPersonUOWMapper : IMapper<App.DAL.DTO.DepartmentPerson, App.Domain.DepartmentPerson>
{
    public App.DAL.DTO.DepartmentPerson? Map(App.Domain.DepartmentPerson? entity)
    {
        if (entity == null) return null;
        
        var res = new App.DAL.DTO.DepartmentPerson()
        {
            Id = entity.Id,
            
            DepartmentId = entity.DepartmentId,
            Department = entity.Department != null ? new App.DAL.DTO.Department()
            {
                Id = entity.Department.Id,
                DepartmentName = entity.Department.DepartmentName
            } : null,
            
            PersonId = entity.PersonId,
            Person = entity.Person != null ? new App.DAL.DTO.Person()
            {
                Id = entity.Person.Id,
                PersonName = entity.Person.PersonName
            } : null,
            
            Position = entity.Position
        };
        
        return res;
    }

    public App.Domain.DepartmentPerson? Map(App.DAL.DTO.DepartmentPerson? entity)
    {
        if (entity == null) return null;
        
        var res = new App.Domain.DepartmentPerson()
        {
            Id = entity.Id,
            
            DepartmentId = entity.DepartmentId,
            Department = entity.Department != null ? new App.Domain.Department()
            {
                Id = entity.Department.Id,
                DepartmentName = entity.Department.DepartmentName
            } : null,
            
            PersonId = entity.PersonId,
            Person = entity.Person != null ? new App.Domain.Person()
            {
                Id = entity.Person.Id,
                PersonName = entity.Person.PersonName
            } : null,
            
            Position = entity.Position
        };
        
        return res;
    }
}