using Base.BLL.Contracts;

namespace App.BLL.Contracts;

public interface ITaskService : IBaseService<App.BLL.DTO.Task>, ITaskServiceCustom
{
}

public interface ITaskServiceCustom
{
    Task<IEnumerable<App.BLL.DTO.Task>> GetAllWithRelationsAsync(bool noTracking = true);
    Task<App.BLL.DTO.Task?> GetWithRelationsAsync(Guid id, bool noTracking = true);
    Task<IEnumerable<App.BLL.DTO.Task>> GetByTaskKeeperAsync(Guid taskKeeperId, bool noTracking = true);
    Task<IEnumerable<App.BLL.DTO.Task>> GetByDepartmentAsync(Guid departmentId, bool noTracking = true);
    Task<IEnumerable<App.BLL.DTO.Task>> GetByStatusAsync(App.BLL.DTO.TaskStatus status, bool noTracking = true);
    Task<IEnumerable<App.BLL.DTO.Task>> GetByPriorityAsync(int priority, bool noTracking = true);
    Task<IEnumerable<App.BLL.DTO.Task>> GetOverdueTasksAsync(bool noTracking = true);
    Task<IEnumerable<App.BLL.DTO.Task>> GetTasksDueSoonAsync(int daysAhead = 7, bool noTracking = true);
    Task<IEnumerable<App.BLL.DTO.Task>> GetByCreatorAsync(string createdBy, bool noTracking = true);
    
    // Business logic methods
    Task<App.BLL.DTO.Task?> CompleteTaskAsync(Guid taskId, string? completionNotes = null);
    Task<App.BLL.DTO.Task?> AssignTaskAsync(Guid taskId, Guid taskKeeperId);
    Task<App.BLL.DTO.Task?> ChangeTaskStatusAsync(Guid taskId, App.BLL.DTO.TaskStatus newStatus);
    Task<App.BLL.DTO.Task?> UpdateTaskPriorityAsync(Guid taskId, int newPriority);
    
    // Statistics
    Task<TaskStatistics> GetTaskStatisticsAsync();
}

// TaskStatistics class for task analytics
public class TaskStatistics
{
    public int TotalTasks { get; set; }
    public int OpenTasks { get; set; }
    public int InProgressTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int OverdueTasks { get; set; }
    public double CompletionRate { get; set; }
    public int HighPriorityTasks { get; set; }
    public int TasksDueSoon { get; set; }
}