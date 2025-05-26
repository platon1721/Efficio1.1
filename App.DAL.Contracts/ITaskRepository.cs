using Base.DAL.Contracts;

namespace App.DAL.Contracts;

public interface ITaskRepository: IBaseRepository<App.DAL.DTO.Task>, ITaskRepositoryCustom
{
    
}

public interface ITaskRepositoryCustom
{
    Task<IEnumerable<App.DAL.DTO.Task>> GetAllWithRelationsAsync(bool noTracking = true);
    Task<App.DAL.DTO.Task?> GetWithRelationsAsync(Guid id, bool noTracking = true);
    Task<IEnumerable<App.DAL.DTO.Task>> GetByTaskKeeperAsync(Guid taskKeeperId, bool noTracking = true);
    Task<IEnumerable<App.DAL.DTO.Task>> GetByDepartmentAsync(Guid departmentId, bool noTracking = true);
    Task<IEnumerable<App.DAL.DTO.Task>> GetByStatusAsync(App.DAL.DTO.TaskStatus status, bool noTracking = true);
    Task<IEnumerable<App.DAL.DTO.Task>> GetByPriorityAsync(int priority, bool noTracking = true);
    Task<IEnumerable<App.DAL.DTO.Task>> GetOverdueTasksAsync(bool noTracking = true);
    Task<IEnumerable<App.DAL.DTO.Task>> GetTasksDueSoonAsync(int daysAhead = 7, bool noTracking = true);
    Task<IEnumerable<App.DAL.DTO.Task>> GetByCreatorAsync(string createdBy, bool noTracking = true);
}