using App.DAL.DTO;
using Base.Contracts;

namespace App.DAL.EF.Mappers;

public class PersonUOWMapper : IMapper<App.DAL.DTO.Person, App.Domain.Person>
{
    public Person? Map(Domain.Person? entity)
    {
        if (entity == null) return null;
        
        var res = new Person()
        {
            Id = entity.Id,
            PersonName = entity.PersonName,
            Contacts = entity.Contacts?.Select(c => new Contact()
            {
                Id = c.Id,
                Value = c.Value,
                ContactTypeId = c.ContactTypeId,
                ContactType = null,
                PersonId = c.PersonId,
                Person = null
            }).ToList(),
            
            // Map DepartmentPersons
            DepartmentPersons = entity.DepartmentPersons?.Select(dp => new DepartmentPerson()
            {
                Id = dp.Id,
                DepartmentId = dp.DepartmentId,
                Department = dp.Department != null ? new Department()
                {
                    Id = dp.Department.Id,
                    DepartmentName = dp.Department.DepartmentName
                } : null,
                PersonId = dp.PersonId,
                Position = dp.Position
            }).ToList(),
            
            // Map Departments from DepartmentPersons
            Departments = entity.DepartmentPersons?.Select(dp => dp.Department)
                                .Where(d => d != null)
                                .Select(d => new Department()
                                {
                                    Id = d!.Id,
                                    DepartmentName = d.DepartmentName
                                }).ToList()
        };
        
        return res;
    }

    public Domain.Person? Map(Person? entity)
    {
        if (entity == null) return null;
        
        var res = new Domain.Person()
        {
            Id = entity.Id,
            PersonName = entity.PersonName,
            Contacts = entity.Contacts?.Select(c => new Domain.Contact()
            {
                Id = c.Id,
                Value = c.Value,
                ContactTypeId = c.ContactTypeId,
                ContactType = null,
                PersonId = c.PersonId,
                Person = null
            }).ToList(),
            
            // Map DepartmentPersons
            DepartmentPersons = entity.DepartmentPersons?.Select(dp => new Domain.DepartmentPerson()
            {
                Id = dp.Id,
                DepartmentId = dp.DepartmentId,
                Department = dp.Department != null ? new Domain.Department()
                {
                    Id = dp.Department.Id,
                    DepartmentName = dp.Department.DepartmentName
                } : null,
                PersonId = dp.PersonId,
                Position = dp.Position
            }).ToList()
            
            // Note: We don't map Departments directly as it's a calculated property
        };
        
        return res;
    }
}