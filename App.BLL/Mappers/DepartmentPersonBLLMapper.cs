using Base.Contracts;

namespace App.BLL.Mappers;

public class DepartmentPersonBLLMapper : IMapper<App.BLL.DTO.DepartmentPerson, App.DAL.DTO.DepartmentPerson>
{
    public App.BLL.DTO.DepartmentPerson? Map(App.DAL.DTO.DepartmentPerson? entity)
    {
        if (entity == null) return null;
        
        var res = new App.BLL.DTO.DepartmentPerson()
        {
            Id = entity.Id,
            
            DepartmentId = entity.DepartmentId,
            Department = entity.Department != null ? new App.BLL.DTO.Department()
            {
                Id = entity.Department.Id,
                DepartmentName = entity.Department.DepartmentName
            } : null,
            
            PersonId = entity.PersonId,
            Person = entity.Person != null ? new App.BLL.DTO.Person()
            {
                Id = entity.Person.Id,
                PersonName = entity.Person.PersonName
            } : null,
            
            Position = entity.Position
        };
        
        return res;
    }

    public App.DAL.DTO.DepartmentPerson? Map(App.BLL.DTO.DepartmentPerson? entity)
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
}