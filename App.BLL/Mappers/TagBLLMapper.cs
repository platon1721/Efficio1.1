using App.BLL.DTO;
using Base.Contracts;

namespace App.BLL.Mappers;

public class TagBLLMapper : IMapper<App.BLL.DTO.Tag, App.DAL.DTO.Tag>
{
    public App.BLL.DTO.Tag? Map(App.DAL.DTO.Tag? entity)
    {
        if (entity == null) return null;
        
        return new App.BLL.DTO.Tag
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description
        };
    }

    public App.DAL.DTO.Tag? Map(App.BLL.DTO.Tag? entity)
    {
        if (entity == null) return null;
        
        return new App.DAL.DTO.Tag
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description
        };
    }
}