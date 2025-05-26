using App.BLL.DTO;
using Base.BLL.Contracts;

namespace App.BLL.Contracts;

public interface IDepartmentService : IBaseService<Department>
{
    Task<IEnumerable<Department>> GetAllWithManagerAsync(bool noTracking = true);
    Task<Department?> GetWithPersonsAsync(Guid id, bool noTracking = true);
    Task<Department?> GetWithTasksAsync(Guid id, bool noTracking = true);
    Task<Department?> GetWithAllRelationsAsync(Guid id, bool noTracking = true);
    Task<IEnumerable<Department>> GetAllWithTasksAsync(bool noTracking = true);
}