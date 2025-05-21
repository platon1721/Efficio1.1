using App.DAL.DTO;
using Base.Contracts;
using Base.DAL.Contracts;

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
                // TODO: Add ContactType mapping
                ContactType = null,

                PersonId = c.PersonId,
                // TODO: Add Person mapping
                Person = null
                
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
                // TODO: Add ContactType mapping
                ContactType = null,

                PersonId = c.PersonId,
                // TODO: Add Person mapping
                Person = null
                
            }).ToList()
        };
        return res;
    }
}