using Base.BLL.Contracts;

namespace App.BLL.Contracts;

public interface IPostService : IBaseService<App.BLL.DTO.Post>
{
    // Existing repository methods - return BLL DTOs
    Task<IEnumerable<App.BLL.DTO.Post>> GetAllWithTagsAndDepartmentsAsync(bool noTracking = true);
    Task<App.BLL.DTO.Post?> GetWithTagsAndDepartmentsAsync(Guid id, bool noTracking = true);
    Task<IEnumerable<App.BLL.DTO.Post>> GetByTagAsync(Guid tagId, bool noTracking = true);
    Task<IEnumerable<App.BLL.DTO.Post>> GetByDepartmentAsync(Guid departmentId, bool noTracking = true);
    
    Task<IEnumerable<App.BLL.DTO.Post>> GetAllWithTagsDepartmentsAndCommentsAsync(bool noTracking = true);
    Task<App.BLL.DTO.Post?> GetWithTagsDepartmentsAndCommentsAsync(Guid id, bool noTracking = true);
    Task<IEnumerable<App.BLL.DTO.Post>> GetWithCommentsAsync(bool noTracking = true);
    Task<App.BLL.DTO.Post?> GetPostWithCommentsAsync(Guid id, bool noTracking = true);
    
    Task<App.BLL.DTO.Post?> CreatePostWithTagsAndDepartmentsAsync(
        App.BLL.DTO.Post post, 
        IEnumerable<Guid> tagIds, 
        IEnumerable<Guid> departmentIds);
    
    Task<bool> UpdatePostTagsAsync(Guid postId, IEnumerable<Guid> tagIds);
    Task<bool> UpdatePostDepartmentsAsync(Guid postId, IEnumerable<Guid> departmentIds);
    
    Task<bool> AddTagToPostAsync(Guid postId, Guid tagId);
    Task<bool> RemoveTagFromPostAsync(Guid postId, Guid tagId);
    
    Task<bool> AddDepartmentToPostAsync(Guid postId, Guid departmentId);
    Task<bool> RemoveDepartmentFromPostAsync(Guid postId, Guid departmentId);
    
    Task<IEnumerable<App.BLL.DTO.Comment>> GetPostCommentsAsync(Guid postId, bool noTracking = true);
    Task<App.BLL.DTO.Comment?> AddCommentToPostAsync(Guid postId, string content);
    Task<bool> RemoveCommentFromPostAsync(Guid postId, Guid commentId);
    Task<int> GetPostCommentCountAsync(Guid postId);
}