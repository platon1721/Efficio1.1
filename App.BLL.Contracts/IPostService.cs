using Base.BLL.Contracts;
using App.DAL.Contracts;

namespace App.BLL.Contracts;

public interface IPostService : IBaseService<App.BLL.DTO.Post>, IPostRepositoryCustom
{
    // Additional BLL-specific methods
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
}