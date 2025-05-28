using Base.Contracts;

namespace App.DTO.v1.Mappers;

public class TagMapper : IMapper<App.DTO.v1.Tag, App.BLL.DTO.Tag>
{
    public App.DTO.v1.Tag? Map(App.BLL.DTO.Tag? entity)
    {
        if (entity == null) return null;
        
        return new App.DTO.v1.Tag
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description
        };
    }

    public App.BLL.DTO.Tag? Map(App.DTO.v1.Tag? entity)
    {
        if (entity == null) return null;
        
        return new App.BLL.DTO.Tag
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description
        };
    }
    
    public App.BLL.DTO.Tag Map(App.DTO.v1.CreateTag entity)
    {
        return new App.BLL.DTO.Tag
        {
            Id = Guid.NewGuid(),
            Title = entity.Title,
            Description = entity.Description
        };
    }
}