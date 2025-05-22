using Base.DAL.Contracts;

namespace App.DAL.Contracts;

public interface IPostTagRepository : IBaseRepository<App.DAL.DTO.PostTag>, IPostTagRepositoryCustom
{
}

public interface IPostTagRepositoryCustom
{
    Task<IEnumerable<App.DAL.DTO.PostTag>> GetByPostIdAsync(Guid postId, bool noTracking = true);
    Task<IEnumerable<App.DAL.DTO.PostTag>> GetByTagIdAsync(Guid tagId, bool noTracking = true);
    Task<bool> PostTagExistsAsync(Guid postId, Guid tagId);
    Task RemoveByPostAndTagAsync(Guid postId, Guid tagId);
}