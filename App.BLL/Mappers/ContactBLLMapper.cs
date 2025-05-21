using App.DAL.DTO;
using Base.BLL.Contracts;
using Base.Contracts;

namespace App.BLL.Mappers;

public class ContactBLLMapper : IMapper<App.BLL.DTO.Contact, App.DAL.DTO.Contact>
{
    public App.DAL.DTO.Contact? Map(App.BLL.DTO.Contact? entity)
    {
        if (entity == null) return null;
        var res = new App.DAL.DTO.Contact()
        {
            Id = entity.Id,
            Value = entity.Value,
            ContactTypeId = entity.ContactTypeId,
            PersonId = entity.PersonId,
        };

        return res;
    }

    public App.BLL.DTO.Contact? Map(App.DAL.DTO.Contact? entity)
    {
        if (entity == null) return null;
        var res = new App.BLL.DTO.Contact()
        {
            Id = entity.Id,
            Value = entity.Value,
            ContactTypeId = entity.ContactTypeId,
            ContactType = entity.ContactType == null
                ? null
                : new App.BLL.DTO.ContactType()
                {
                    Id = entity.ContactType.Id,
                    ContactTypeName = entity.ContactType.ContactTypeName,
                },

            PersonId = entity.PersonId,
            Person = entity.Person == null
                ? null
                : new App.BLL.DTO.Person()
                {
                    Id = entity.Person!.Id,
                    PersonName = entity.Person!.PersonName,
                }
        };

        return res;
    }
}