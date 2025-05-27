using Base.DAL.Contracts;

namespace App.DAL.Contracts;

public interface IFeedbackRepository : IBaseRepository<App.DAL.DTO.Feedback>, IFeedbackRepositoryCustom
{
}

public interface IFeedbackRepositoryCustom
{
    Task<IEnumerable<App.DAL.DTO.Feedback>> GetAllWithTagsAndCommentsAsync(bool noTracking = true);
    Task<App.DAL.DTO.Feedback?> GetWithTagsAndCommentsAsync(Guid id, bool noTracking = true);
    Task<IEnumerable<App.DAL.DTO.Feedback>> GetByDepartmentAsync(Guid departmentId, bool noTracking = true);
    Task<IEnumerable<App.DAL.DTO.Feedback>> GetByTagAsync(Guid tagId, bool noTracking = true);
}