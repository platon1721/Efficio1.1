using Base.DAL.Contracts;

namespace App.DAL.Contracts;

public interface ICommentRepository : IBaseRepository<App.DAL.DTO.Comment>, ICommentRepositoryCustom
{
}

public interface ICommentRepositoryCustom
{
    Task<IEnumerable<App.DAL.DTO.Comment>> GetByPostIdAsync(Guid postId, bool noTracking = true);
    Task<IEnumerable<App.DAL.DTO.Comment>> GetByFeedbackIdAsync(Guid feedbackId, bool noTracking = true);
}