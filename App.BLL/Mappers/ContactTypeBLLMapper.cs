using App.DAL.DTO;
using Base.Contracts;

namespace App.BLL.Mappers;

public class ContactTypeBLLMapper : IMapper<App.BLL.DTO.ContactType, App.DAL.DTO.ContactType>
{
    public App.DAL.DTO.ContactType? Map(App.BLL.DTO.ContactType? entity)
    {
        if (entity == null) return null;
        var res = new App.DAL.DTO.ContactType()
        {
            Id = entity.Id,
            ContactTypeName = entity.ContactTypeName
        };

        return res;
    }

    public App.BLL.DTO.ContactType? Map(App.DAL.DTO.ContactType? entity)
    {
        if (entity == null) return null;
        var res = new App.BLL.DTO.ContactType()
        {
            Id = entity.Id,
            ContactTypeName = entity.ContactTypeName
        };
        return res;
    }
}