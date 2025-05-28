using Base.Contracts;

namespace App.DTO.v1.Mappers;

public class PostMapper : IMapper<App.DTO.v1.Post, App.BLL.DTO.Post>
{
    public App.DTO.v1.Post? Map(App.BLL.DTO.Post? entity)
    {
        if (entity == null) return null;
        
        return new App.DTO.v1.Post
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description
        };
    }

    public App.BLL.DTO.Post? Map(App.DTO.v1.Post? entity)
    {
        if (entity == null) return null;
        
        return new App.BLL.DTO.Post
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description
        };
    }
    
    public App.BLL.DTO.Post Map(App.DTO.v1.CreatePost entity)
    {
        return new App.BLL.DTO.Post
        {
            Id = Guid.NewGuid(),
            Title = entity.Title,
            Description = entity.Description
        };
    }
}