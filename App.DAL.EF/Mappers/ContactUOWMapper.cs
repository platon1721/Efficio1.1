using App.DAL.DTO;
using Base.Contracts;
using Base.DAL.Contracts;

namespace App.DAL.EF.Mappers;

public class ContactUOWMapper : IMapper<App.DAL.DTO.Contact, App.Domain.Contact>
{
    public App.DAL.DTO.Contact? Map(App.Domain.Contact? entity)
    {
        if (entity == null) return null;

        var res = new App.DAL.DTO.Contact()
        {
            Id = entity.Id,
            Value = entity.Value,
            ContactTypeId = entity.ContactTypeId,
            ContactType = entity.ContactType == null
                ? null
                : new ContactType()
                {
                    Id = entity.ContactType.Id,
                    ContactTypeName = entity.ContactType.ContactTypeName,
                },
            PersonId = entity.PersonId,
            Person = entity.Person == null
                ? null
                : new Person()
                {
                    Id = entity.Person!.Id,
                    PersonName = entity.Person!.PersonName,
                }
        };

        return res;
    }

    public App.Domain.Contact? Map(App.DAL.DTO.Contact? entity)
    {
        if (entity == null) return null;

        var res = new App.Domain.Contact()
        {
            Id = entity.Id,
            Value = entity.Value,
            ContactTypeId = entity.ContactTypeId,
            PersonId = entity.PersonId,
        };

        return res;
    }
}