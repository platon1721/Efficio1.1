using App.DAL.DTO;
using Base.Contracts;

namespace App.DAL.EF.Mappers;

public class TagUOWMapper: IMapper<App.DAL.DTO.Tag, App.Domain.Tag>
{
    public App.DAL.DTO.Tag? Map(Domain.Tag? entity)
    {
        if (entity == null) return null;
        return new App.DAL.DTO.Tag()
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
        };
    }

    public App.Domain.Tag? Map(Tag? entity)
    {
        if (entity == null) return null;
        return new App.Domain.Tag()
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description
        };
    }
}