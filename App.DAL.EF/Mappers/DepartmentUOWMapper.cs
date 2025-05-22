using Base.Contracts;

namespace App.DAL.EF.Mappers;

public class DepartmentUOWMapper : IMapper<App.DAL.DTO.Department, App.Domain.Department>
{
    public App.DAL.DTO.Department? Map(App.Domain.Department? entity)
    {
        if (entity == null) return null;
        
        var res = new App.DAL.DTO.Department()
        {
            Id = entity.Id,
            DepartmentName = entity.DepartmentName,
            
            ManagerId = entity.ManagerId,
            Manager = entity.Manager != null ? new App.DAL.DTO.Person()
            {
                Id = entity.Manager.Id,
                PersonName = entity.Manager.PersonName
            } : null,
            
            // Map persons from DepartmentPersons
            Persons = entity.DepartmentPersons?.Select(dp => dp.Person)
                .Where(p => p != null)
                .Select(p => new App.DAL.DTO.Person()
                {
                    Id = p!.Id,
                    PersonName = p.PersonName
                }).ToList()
        };
        
        return res;
    }

    public App.Domain.Department? Map(App.DAL.DTO.Department? entity)
    {
        if (entity == null) return null;
        
        var res = new App.Domain.Department()
        {
            Id = entity.Id,
            DepartmentName = entity.DepartmentName,
            
            ManagerId = entity.ManagerId,
            Manager = entity.Manager != null ? new App.Domain.Person()
            {
                Id = entity.Manager.Id,
                PersonName = entity.Manager.PersonName
            } : null,
            
            // We don't map Persons to DepartmentPersons here since it's a computed property
            // The DepartmentPersons will be handled separately
        };
        
        return res;
    }
}