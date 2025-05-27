using Base.BLL.Contracts;

namespace App.BLL.Contracts;

public interface IFeedbackTagService : IBaseService<App.BLL.DTO.FeedbackTag>
{
    Task<IEnumerable<App.BLL.DTO.FeedbackTag>> GetByFeedbackIdAsync(Guid feedbackId, bool noTracking = true);
    Task<IEnumerable<App.BLL.DTO.FeedbackTag>> GetByTagIdAsync(Guid tagId, bool noTracking = true);
    Task<bool> FeedbackTagExistsAsync(Guid feedbackId, Guid tagId);
    Task RemoveByFeedbackAndTagAsync(Guid feedbackId, Guid tagId);
}