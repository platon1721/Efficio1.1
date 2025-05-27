using Base.BLL.Contracts;

namespace App.BLL.Contracts;

public interface IFeedbackService : IBaseService<App.BLL.DTO.Feedback>, IFeedbackServiceCustom
{
}

public interface IFeedbackServiceCustom
{
    Task<IEnumerable<App.BLL.DTO.Feedback>> GetAllWithTagsAndCommentsAsync(bool noTracking = true);
    Task<App.BLL.DTO.Feedback?> GetWithTagsAndCommentsAsync(Guid id, bool noTracking = true);
    Task<IEnumerable<App.BLL.DTO.Feedback>> GetByDepartmentAsync(Guid departmentId, bool noTracking = true);
    Task<IEnumerable<App.BLL.DTO.Feedback>> GetByTagAsync(Guid tagId, bool noTracking = true);
    
    // BLL-specific methods
    Task<App.BLL.DTO.Feedback?> CreateFeedbackWithTagsAsync(
        App.BLL.DTO.Feedback feedback, 
        IEnumerable<Guid> tagIds);
    
    Task<bool> UpdateFeedbackTagsAsync(Guid feedbackId, IEnumerable<Guid> tagIds);
    
    Task<bool> AddTagToFeedbackAsync(Guid feedbackId, Guid tagId);
    Task<bool> RemoveTagFromFeedbackAsync(Guid feedbackId, Guid tagId);
}