using Base.Contracts;

namespace App.DTO.v1.Mappers;

public class ContactTypeMapper : IMapper<App.DTO.v1.ContactType, App.BLL.DTO.ContactType>
{
    public App.DTO.v1.ContactType? Map(BLL.DTO.ContactType? entity)
    {
        if (entity == null) return null;
        var res = new App.DTO.v1.ContactType()
        {
            Id = entity.Id,
            ContactTypeName = entity.ContactTypeName
        };
        return res;
    }

    public BLL.DTO.ContactType? Map(ContactType? entity)
    {
        if (entity == null) return null;
        var res = new BLL.DTO.ContactType()
        {
            Id = entity.Id,
            ContactTypeName = entity.ContactTypeName
        };
        return res;
    }
    
    public BLL.DTO.ContactType Map(ContactTypeCreate entity)
    {
        var res = new BLL.DTO.ContactType()
        {
            Id = Guid.NewGuid(),
            ContactTypeName = entity.ContactTypeName
        };
        return res;
    }
}