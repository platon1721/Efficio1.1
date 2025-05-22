using Base.BLL.Contracts;

namespace App.BLL.Contracts;

public interface IPostTagService : IBaseService<App.BLL.DTO.PostTag>
{
    Task<IEnumerable<App.BLL.DTO.PostTag>> GetByPostIdAsync(Guid postId, bool noTracking = true);
    Task<IEnumerable<App.BLL.DTO.PostTag>> GetByTagIdAsync(Guid tagId, bool noTracking = true);
    Task<bool> PostTagExistsAsync(Guid postId, Guid tagId);
    Task RemoveByPostAndTagAsync(Guid postId, Guid tagId);
}