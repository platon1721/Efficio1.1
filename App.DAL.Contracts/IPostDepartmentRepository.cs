using Base.DAL.Contracts;

namespace App.DAL.Contracts;

public interface IPostDepartmentRepository : IBaseRepository<App.DAL.DTO.PostDepartment>, IPostDepartmentRepositoryCustom
{
}

public interface IPostDepartmentRepositoryCustom
{
    Task<IEnumerable<App.DAL.DTO.PostDepartment>> GetByPostIdAsync(Guid postId, bool noTracking = true);
    Task<IEnumerable<App.DAL.DTO.PostDepartment>> GetByDepartmentIdAsync(Guid departmentId, bool noTracking = true);
    Task<bool> PostDepartmentExistsAsync(Guid postId, Guid departmentId);
    Task RemoveByPostAndDepartmentAsync(Guid postId, Guid departmentId);
}