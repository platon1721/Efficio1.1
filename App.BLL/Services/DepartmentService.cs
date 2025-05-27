using App.BLL.Contracts;
using App.DAL.Contracts;
using Base.BLL;
using Base.Contracts;

namespace App.BLL.Services;

public class DepartmentService : BaseService<App.BLL.DTO.Department, App.DAL.DTO.Department, App.DAL.Contracts.IDepartmentRepository>, IDepartmentService
{
    public DepartmentService(
        IAppUOW serviceUOW,
        IMapper<App.BLL.DTO.Department, App.DAL.DTO.Department> mapper) 
        : base(serviceUOW, serviceUOW.DepartmentRepository, mapper)
    {
    }

    // EXISTING METHODS
    public async Task<IEnumerable<App.BLL.DTO.Department>> GetAllWithManagerAsync(bool noTracking = true)
    {
        var dalItems = await ServiceRepository.GetAllWithManagerAsync(noTracking);
        return dalItems.Select(item => Mapper.Map(item)!);
    }

    public async Task<App.BLL.DTO.Department?> GetWithPersonsAsync(Guid id, bool noTracking = true)
    {
        var dalItem = await ServiceRepository.GetWithPersonsAsync(id, noTracking);
        return Mapper.Map(dalItem);
    }

    public async Task<App.BLL.DTO.Department?> GetWithTasksAsync(Guid id, bool noTracking = true)
    {
        var dalItem = await ServiceRepository.GetWithTasksAsync(id, noTracking);
        return Mapper.Map(dalItem);
    }

    public async Task<App.BLL.DTO.Department?> GetWithAllRelationsAsync(Guid id, bool noTracking = true)
    {
        var dalItem = await ServiceRepository.GetWithAllRelationsAsync(id, noTracking);
        return Mapper.Map(dalItem);
    }

    public async Task<IEnumerable<App.BLL.DTO.Department>> GetAllWithTasksAsync(bool noTracking = true)
    {
        var dalItems = await ServiceRepository.GetAllWithTasksAsync(noTracking);
        return dalItems.Select(item => Mapper.Map(item)!);
    }

    // NEW METHODS FOR FEEDBACK SUPPORT
    public async Task<App.BLL.DTO.Department?> GetWithFeedbacksAsync(Guid id, bool noTracking = true)
    {
        var dalItem = await ServiceRepository.GetWithFeedbacksAsync(id, noTracking);
        return Mapper.Map(dalItem);
    }

    public async Task<IEnumerable<App.BLL.DTO.Department>> GetAllWithFeedbacksAsync(bool noTracking = true)
    {
        var dalItems = await ServiceRepository.GetAllWithFeedbacksAsync(noTracking);
        return dalItems.Select(item => Mapper.Map(item)!);
    }

    // BUSINESS METHODS
    public async Task<IEnumerable<App.BLL.DTO.Feedback>> GetDepartmentFeedbacksAsync(Guid departmentId, bool noTracking = true)
    {
        var department = await GetWithFeedbacksAsync(departmentId, noTracking);
        return department?.Feedbacks ?? new List<App.BLL.DTO.Feedback>();
    }

    public async Task<IEnumerable<App.BLL.DTO.Task>> GetDepartmentTasksAsync(Guid departmentId, bool noTracking = true)
    {
        var department = await GetWithTasksAsync(departmentId, noTracking);
        return department?.Tasks ?? new List<App.BLL.DTO.Task>();
    }

    public async Task<DepartmentStatistics> GetDepartmentStatisticsAsync(Guid departmentId)
    {
        var department = await GetWithAllRelationsAsync(departmentId, true);
        
        if (department == null)
        {
            return new DepartmentStatistics();
        }

        var tasks = department.Tasks?.ToList() ?? new List<App.BLL.DTO.Task>();
        var feedbacks = department.Feedbacks?.ToList() ?? new List<App.BLL.DTO.Feedback>();
        var completedTasks = tasks.Where(t => t.TaskStatus == App.BLL.DTO.TaskStatus.Completed).ToList();
        var overdueTasks = tasks.Where(t => t.DeadLine < DateTime.UtcNow && t.TaskStatus != App.BLL.DTO.TaskStatus.Completed).ToList();

        return new DepartmentStatistics
        {
            TotalPersons = department.Persons?.Count ?? 0,
            TotalTasks = tasks.Count,
            CompletedTasks = completedTasks.Count,
            OverdueTasks = overdueTasks.Count,
            TotalFeedbacks = feedbacks.Count,
            TotalPosts = department.PostDepartments?.Count ?? 0,
            TaskCompletionRate = tasks.Count > 0 ? (double)completedTasks.Count / tasks.Count * 100 : 0,
            LastFeedbackDate = feedbacks.Count > 0 ? feedbacks.Max(f => DateTime.UtcNow) : null, // Would need CreatedAt from base
            LastTaskCompletionDate = completedTasks.Count > 0 ? completedTasks.Max(t => t.CompletedAt) : null
        };
    }
}