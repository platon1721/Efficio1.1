using Base.DAL.Contracts;

namespace App.DAL.Contracts;

public interface IDepartmentRepository : IBaseRepository<App.DAL.DTO.Department>, IDepartmentRepositoryCustom
{
}

public interface IDepartmentRepositoryCustom
{
    Task<IEnumerable<App.DAL.DTO.Department>> GetAllWithManagerAsync(bool noTracking = true);
    Task<App.DAL.DTO.Department?> GetWithPersonsAsync(Guid id, bool noTracking = true);
    Task<App.DAL.DTO.Department?> GetWithTasksAsync(Guid id, bool noTracking = true);
    Task<App.DAL.DTO.Department?> GetWithAllRelationsAsync(Guid id, bool noTracking = true);
    Task<IEnumerable<App.DAL.DTO.Department>> GetAllWithTasksAsync(bool noTracking = true);
    
    // Task<IEnumerable<App.DAL.DTO.Department>> GetAllWithManagerAsync(bool noTracking = true);
    // Task<App.DAL.DTO.Department?> GetWithPersonsAsync(Guid id, bool noTracking = true);
    // Task<App.DAL.DTO.Department?> GetWithTasksAsync(Guid id, bool noTracking = true);
    // Task<App.DAL.DTO.Department?> GetWithAllRelationsAsync(Guid id, bool noTracking = true);
    // Task<IEnumerable<App.DAL.DTO.Department>> GetAllWithTasksAsync(bool noTracking = true);
    // Task<IEnumerable<App.DAL.DTO.Task>> GetDepartmentTasksAsync(Guid departmentId, bool noTracking = true);
    // Task<IEnumerable<App.DAL.DTO.Task>> GetDepartmentTasksByStatusAsync(Guid departmentId, App.DAL.DTO.TaskStatus status, bool noTracking = true);
    // Task<IEnumerable<App.DAL.DTO.Task>> GetDepartmentOverdueTasksAsync(Guid departmentId, bool noTracking = true);
}