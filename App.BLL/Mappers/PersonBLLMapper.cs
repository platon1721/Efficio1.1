using Base.Contracts;

namespace App.BLL.Mappers;

public class PersonBLLMapper : IMapper<App.BLL.DTO.Person, App.DAL.DTO.Person>
{
    public App.BLL.DTO.Person? Map(App.DAL.DTO.Person? entity)
    {
        if (entity == null) return null;
        
        var res = new App.BLL.DTO.Person()
        {
            Id = entity.Id,
            PersonName = entity.PersonName,
            
            Contacts = entity.Contacts?.Select(c => new App.BLL.DTO.Contact()
            {
                Id = c.Id,
                Value = c.Value,
                ContactTypeId = c.ContactTypeId,
                PersonId = c.PersonId
                // Map other properties as needed
            }).ToList(),
            
            // Map Departments
            Departments = entity.Departments?.Select(d => new App.BLL.DTO.Department()
            {
                Id = d.Id,
                DepartmentName = d.DepartmentName
                // Map other properties as needed
            }).ToList(),
            
            // Optionally map DepartmentPersons if needed
            DepartmentPersons = entity.DepartmentPersons?.Select(dp => new App.BLL.DTO.DepartmentPerson()
            {
                Id = dp.Id,
                DepartmentId = dp.DepartmentId,
                PersonId = dp.PersonId,
                Position = dp.Position
                // Map other properties as needed
            }).ToList()
        };
        
        return res;
    }

    public App.DAL.DTO.Person? Map(App.BLL.DTO.Person? entity)
    {
        if (entity == null) return null;
        
        var res = new App.DAL.DTO.Person()
        {
            Id = entity.Id,
            PersonName = entity.PersonName,
            
            Contacts = entity.Contacts?.Select(c => new App.DAL.DTO.Contact()
            {
                Id = c.Id,
                Value = c.Value,
                ContactTypeId = c.ContactTypeId,
                PersonId = c.PersonId
                // Map other properties as needed
            }).ToList(),
            
            // Map Departments
            Departments = entity.Departments?.Select(d => new App.DAL.DTO.Department()
            {
                Id = d.Id,
                DepartmentName = d.DepartmentName
                // Map other properties as needed
            }).ToList(),
            
            // Optionally map DepartmentPersons if needed
            DepartmentPersons = entity.DepartmentPersons?.Select(dp => new App.DAL.DTO.DepartmentPerson()
            {
                Id = dp.Id,
                DepartmentId = dp.DepartmentId,
                PersonId = dp.PersonId,
                Position = dp.Position
                // Map other properties as needed
            }).ToList()
        };
        
        return res;
    }
}