using Base.DAL.Contracts;

namespace App.DAL.Contracts;

public interface IPostRepository : IBaseRepository<App.DAL.DTO.Post>, IPostRepositoryCustom
{
}

public interface IPostRepositoryCustom
{
    Task<IEnumerable<App.DAL.DTO.Post>> GetAllWithTagsAndDepartmentsAsync(bool noTracking = true);
    Task<App.DAL.DTO.Post?> GetWithTagsAndDepartmentsAsync(Guid id, bool noTracking = true);
    Task<IEnumerable<App.DAL.DTO.Post>> GetByTagAsync(Guid tagId, bool noTracking = true);
    Task<IEnumerable<App.DAL.DTO.Post>> GetByDepartmentAsync(Guid departmentId, bool noTracking = true);
}