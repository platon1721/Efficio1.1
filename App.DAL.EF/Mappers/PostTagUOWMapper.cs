using Base.Contracts;

namespace App.DAL.EF.Mappers;

public class PostTagUOWMapper : IMapper<App.DAL.DTO.PostTag, App.Domain.PostTag>
{
    public App.DAL.DTO.PostTag? Map(App.Domain.PostTag? entity)
    {
        if (entity == null) return null;
        
        return new App.DAL.DTO.PostTag
        {
            Id = entity.Id,
            PostId = entity.PostId,
            TagId = entity.TagId,
            Post = entity.Post != null ? new App.DAL.DTO.Post
            {
                Id = entity.Post.Id,
                Title = entity.Post.Title,
                Description = entity.Post.Description
            } : null,
            Tag = entity.Tag != null ? new App.DAL.DTO.Tag
            {
                Id = entity.Tag.Id,
                Title = entity.Tag.Title,
                Description = entity.Tag.Description
            } : null
        };
    }

    public App.Domain.PostTag? Map(App.DAL.DTO.PostTag? entity)
    {
        if (entity == null) return null;
        
        return new App.Domain.PostTag
        {
            Id = entity.Id,
            PostId = entity.PostId,
            TagId = entity.TagId
        };
    }
}