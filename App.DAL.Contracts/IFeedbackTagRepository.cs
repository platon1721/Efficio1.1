using Base.DAL.Contracts;

namespace App.DAL.Contracts;

public interface IFeedbackTagRepository : IBaseRepository<App.DAL.DTO.FeedbackTag>, IFeedbackTagRepositoryCustom
{
}

public interface IFeedbackTagRepositoryCustom
{
    Task<IEnumerable<App.DAL.DTO.FeedbackTag>> GetByFeedbackIdAsync(Guid feedbackId, bool noTracking = true);
    Task<IEnumerable<App.DAL.DTO.FeedbackTag>> GetByTagIdAsync(Guid tagId, bool noTracking = true);
    Task<bool> FeedbackTagExistsAsync(Guid feedbackId, Guid tagId);
    Task RemoveByFeedbackAndTagAsync(Guid feedbackId, Guid tagId);
}