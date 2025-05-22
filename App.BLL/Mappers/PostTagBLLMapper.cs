using Base.Contracts;

namespace App.BLL.Mappers;

public class PostTagBLLMapper : IMapper<App.BLL.DTO.PostTag, App.DAL.DTO.PostTag>
{
    public App.BLL.DTO.PostTag? Map(App.DAL.DTO.PostTag? entity)
    {
        if (entity == null) return null;
        
        return new App.BLL.DTO.PostTag
        {
            Id = entity.Id,
            PostId = entity.PostId,
            TagId = entity.TagId,
            Post = entity.Post != null ? new App.BLL.DTO.Post
            {
                Id = entity.Post.Id,
                Title = entity.Post.Title,
                Description = entity.Post.Description
            } : null,
            Tag = entity.Tag != null ? new App.BLL.DTO.Tag
            {
                Id = entity.Tag.Id,
                Title = entity.Tag.Title,
                Description = entity.Tag.Description
            } : null
        };
    }

    public App.DAL.DTO.PostTag? Map(App.BLL.DTO.PostTag? entity)
    {
        if (entity == null) return null;
        
        return new App.DAL.DTO.PostTag
        {
            Id = entity.Id,
            PostId = entity.PostId,
            TagId = entity.TagId
        };
    }
}