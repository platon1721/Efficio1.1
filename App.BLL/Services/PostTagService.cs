using App.BLL.Contracts;
using App.BLL.Mappers;
using App.DAL.Contracts;
using Base.BLL;
using Base.Contracts;

namespace App.BLL.Services;

public class PostTagService : BaseService<App.BLL.DTO.PostTag, App.DAL.DTO.PostTag, App.DAL.Contracts.IPostTagRepository>, IPostTagService
{
    public PostTagService(
        IAppUOW serviceUOW,
        IMapper<App.BLL.DTO.PostTag, App.DAL.DTO.PostTag> mapper) 
        : base(serviceUOW, serviceUOW.PostTagRepository, mapper)
    {
    }

    public async Task<IEnumerable<App.DAL.DTO.PostTag>> GetByPostIdAsync(Guid postId, bool noTracking = true)
    {
        return await ServiceRepository.GetByPostIdAsync(postId, noTracking);
    }

    public async Task<IEnumerable<App.DAL.DTO.PostTag>> GetByTagIdAsync(Guid tagId, bool noTracking = true)
    {
        return await ServiceRepository.GetByTagIdAsync(tagId, noTracking);
    }

    public async Task<bool> PostTagExistsAsync(Guid postId, Guid tagId)
    {
        return await ServiceRepository.PostTagExistsAsync(postId, tagId);
    }

    public async Task RemoveByPostAndTagAsync(Guid postId, Guid tagId)
    {
        await ServiceRepository.RemoveByPostAndTagAsync(postId, tagId);
    }
}