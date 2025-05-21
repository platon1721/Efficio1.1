using Base.Contracts;

namespace App.DTO.v1.Mappers;

public class ContactMapper: IMapper<App.DTO.v1.Contact, App.BLL.DTO.Contact>
{
    public App.DTO.v1.Contact? Map(App.BLL.DTO.Contact? entity)
    {
        if (entity == null) return null;
        var res = new App.DTO.v1.Contact()
        {
            Id = entity.Id,
            Value = entity.Value,
            ContactTypeId = entity.ContactTypeId,
            PersonId = entity.PersonId
        };
        return res;
    }

    public App.BLL.DTO.Contact? Map(App.DTO.v1.Contact? entity)
    {
        if (entity == null) return null;
        var res = new App.BLL.DTO.Contact()
        {
            Id = entity.Id,
            Value = entity.Value,
            ContactTypeId = entity.ContactTypeId,
            PersonId = entity.PersonId
        };
        return res;
    }
    
    public App.BLL.DTO.Contact Map(App.DTO.v1.ContactCreate entity)
    {
        var res = new App.BLL.DTO.Contact()
        {
            Id = Guid.NewGuid(),
            Value = entity.Value,
            ContactTypeId = entity.ContactTypeId,
            PersonId = entity.PersonId
        };
        return res;
    }
}