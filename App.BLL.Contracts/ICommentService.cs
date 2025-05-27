using Base.BLL.Contracts;

namespace App.BLL.Contracts;

public interface ICommentService : IBaseService<App.BLL.DTO.Comment>, ICommentServiceCustom
{
}

public interface ICommentServiceCustom
{
    Task<IEnumerable<App.BLL.DTO.Comment>> GetByPostIdAsync(Guid postId, bool noTracking = true);
    Task<IEnumerable<App.BLL.DTO.Comment>> GetByFeedbackIdAsync(Guid feedbackId, bool noTracking = true);
    
    // BLL-specific methods for comment management
    Task<App.BLL.DTO.Comment?> AddCommentToPostAsync(Guid postId, string content);
    Task<App.BLL.DTO.Comment?> AddCommentToFeedbackAsync(Guid feedbackId, string content);
    
    // Business validation
    Task<bool> ValidateCommentAsync(App.BLL.DTO.Comment comment);
}