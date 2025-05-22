using App.DAL.DTO;
using Base.Contracts;

namespace App.DAL.EF.Mappers;

public class TagUOWMapper: IMapper<App.DAL.DTO.Tag, App.Domain.Tag>
{
    public Tag? Map(Domain.Tag? entity)
    {
        throw new NotImplementedException();
    }

    public Domain.Tag? Map(Tag? entity)
    {
        throw new NotImplementedException();
    }
}