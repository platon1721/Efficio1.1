using Base.Contracts;

namespace App.DAL.EF.Mappers;

public class ContactTypeUOWMapper : IMapper<App.DAL.DTO.ContactType, App.Domain.ContactType>
{
    public App.DAL.DTO.ContactType? Map(App.Domain.ContactType? entity)
    {
        if (entity == null) return null;
        var res = new App.DAL.DTO.ContactType()
        {
            Id = entity.Id,
            ContactTypeName = entity.ContactTypeName
        };

        return res;
    }

    public App.Domain.ContactType? Map(App.DAL.DTO.ContactType? entity)
    {
        if (entity == null) return null;
        var res = new App.Domain.ContactType()
        {
            Id = entity.Id,
            ContactTypeName = entity.ContactTypeName
        };
        return res;
    }
}