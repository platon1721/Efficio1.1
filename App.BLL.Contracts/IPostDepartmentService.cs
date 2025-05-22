using Base.BLL.Contracts;

namespace App.BLL.Contracts;

public interface IPostDepartmentService : IBaseService<App.BLL.DTO.PostDepartment>
{
    Task<IEnumerable<App.BLL.DTO.PostDepartment>> GetByPostIdAsync(Guid postId, bool noTracking = true);
    Task<IEnumerable<App.BLL.DTO.PostDepartment>> GetByDepartmentIdAsync(Guid departmentId, bool noTracking = true);
    Task<bool> PostDepartmentExistsAsync(Guid postId, Guid departmentId);
    Task RemoveByPostAndDepartmentAsync(Guid postId, Guid departmentId);
}