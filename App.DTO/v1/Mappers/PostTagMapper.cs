using Base.Contracts;

namespace App.DTO.v1.Mappers;

public class PostTagMapper : IMapper<App.DTO.v1.PostTag, App.BLL.DTO.PostTag>
{
    public App.DTO.v1.PostTag? Map(App.BLL.DTO.PostTag? entity)
    {
        if (entity == null) return null;
        
        return new App.DTO.v1.PostTag
        {
            Id = entity.Id,
            PostId = entity.PostId,
            TagId = entity.TagId,
            PostTitle = entity.Post?.Title,
            TagTitle = entity.Tag?.Title
        };
    }

    public App.BLL.DTO.PostTag? Map(App.DTO.v1.PostTag? entity)
    {
        if (entity == null) return null;
        
        return new App.BLL.DTO.PostTag
        {
            Id = entity.Id,
            PostId = entity.PostId,
            TagId = entity.TagId
        };
    }
}