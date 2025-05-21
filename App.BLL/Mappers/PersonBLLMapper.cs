using App.DAL.DTO;
using Base.BLL.Contracts;
using Base.Contracts;

namespace App.BLL.Mappers;

public class PersonBLLMapper : IMapper<App.BLL.DTO.Person, App.DAL.DTO.Person>
{
    public Person? Map(DTO.Person? entity)
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
        return res;    }

    public DTO.Person? Map(Person? entity)
    {
        if (entity == null) return null;
        var res = new DTO.Person()
        {
            Id = entity.Id,
            PersonName = entity.PersonName,
            Contacts = entity.Contacts?.Select(c => new DTO.Contact()
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