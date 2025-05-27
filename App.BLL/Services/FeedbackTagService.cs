using App.BLL.Contracts;
using App.BLL.Mappers;
using App.DAL.Contracts;
using Base.BLL;
using Base.Contracts;

namespace App.BLL.Services;

public class FeedbackTagService : BaseService<App.BLL.DTO.FeedbackTag, App.DAL.DTO.FeedbackTag, App.DAL.Contracts.IFeedbackTagRepository>, IFeedbackTagService
{
    public FeedbackTagService(
        IAppUOW serviceUOW,
        IMapper<App.BLL.DTO.FeedbackTag, App.DAL.DTO.FeedbackTag> mapper) 
        : base(serviceUOW, serviceUOW.FeedbackTagRepository, mapper)
    {
    }

    public async Task<IEnumerable<App.BLL.DTO.FeedbackTag>> GetByFeedbackIdAsync(Guid feedbackId, bool noTracking = true)
    {
        var dalFeedbackTags = await ServiceRepository.GetByFeedbackIdAsync(feedbackId, noTracking);
        return dalFeedbackTags.Select(ft => Mapper.Map(ft)!);
    }

    public async Task<IEnumerable<App.BLL.DTO.FeedbackTag>> GetByTagIdAsync(Guid tagId, bool noTracking = true)
    {
        var dalFeedbackTags = await ServiceRepository.GetByTagIdAsync(tagId, noTracking);
        return dalFeedbackTags.Select(ft => Mapper.Map(ft)!);
    }

    public async Task<bool> FeedbackTagExistsAsync(Guid feedbackId, Guid tagId)
    {
        return await ServiceRepository.FeedbackTagExistsAsync(feedbackId, tagId);
    }

    public async Task RemoveByFeedbackAndTagAsync(Guid feedbackId, Guid tagId)
    {
        await ServiceRepository.RemoveByFeedbackAndTagAsync(feedbackId, tagId);
    }
}