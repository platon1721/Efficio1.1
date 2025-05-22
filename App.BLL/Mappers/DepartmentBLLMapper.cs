using Base.Contracts;

namespace App.BLL.Mappers;

public class DepartmentBLLMapper : IMapper<App.BLL.DTO.Department, App.DAL.DTO.Department>
{
    public App.BLL.DTO.Department? Map(App.DAL.DTO.Department? entity)
    {
        if (entity == null) return null;
        
        var res = new App.BLL.DTO.Department()
        {
            Id = entity.Id,
            DepartmentName = entity.DepartmentName,
            
            ManagerId = entity.ManagerId,
            Manager = entity.Manager != null ? new App.BLL.DTO.Person()
            {
                Id = entity.Manager.Id,
                PersonName = entity.Manager.PersonName
            } : null,
            
            Persons = entity.Persons?.Select(p => new App.BLL.DTO.Person()
            {
                Id = p.Id,
                PersonName = p.PersonName
            }).ToList()
        };
        
        return res;
    }

    public App.DAL.DTO.Department? Map(App.BLL.DTO.Department? entity)
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
            
            Persons = entity.Persons?.Select(p => new App.DAL.DTO.Person()
            {
                Id = p.Id,
                PersonName = p.PersonName
            }).ToList()
        };
        
        return res;
    }
}