// App.DAL.Contracts/IDepartmentRepository.cs - Updated
using Base.DAL.Contracts;

namespace App.DAL.Contracts;

public interface IDepartmentRepository : IBaseRepository<App.DAL.DTO.Department>, IDepartmentRepositoryCustom
{
}

public interface IDepartmentRepositoryCustom
{
    // Existing methods
    Task<IEnumerable<App.DAL.DTO.Department>> GetAllWithManagerAsync(bool noTracking = true);
    Task<App.DAL.DTO.Department?> GetWithPersonsAsync(Guid id, bool noTracking = true);
    Task<App.DAL.DTO.Department?> GetWithTasksAsync(Guid id, bool noTracking = true);
    Task<App.DAL.DTO.Department?> GetWithAllRelationsAsync(Guid id, bool noTracking = true);
    Task<IEnumerable<App.DAL.DTO.Department>> GetAllWithTasksAsync(bool noTracking = true);
    
    // New methods for Feedback support
    Task<App.DAL.DTO.Department?> GetWithFeedbacksAsync(Guid id, bool noTracking = true);
    Task<IEnumerable<App.DAL.DTO.Department>> GetAllWithFeedbacksAsync(bool noTracking = true);
}