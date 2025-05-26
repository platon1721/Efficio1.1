using App.BLL.Contracts;
using App.DAL.Contracts;
using Base.BLL;
using Base.Contracts;

namespace App.BLL.Services;

public class TaskService : BaseService<App.BLL.DTO.Task, App.DAL.DTO.Task, App.DAL.Contracts.ITaskRepository>, ITaskService
{
    public TaskService(
        IAppUOW serviceUOW,
        IMapper<App.BLL.DTO.Task, App.DAL.DTO.Task> mapper) 
        : base(serviceUOW, serviceUOW.TaskRepository, mapper)
    {
    }

    public async Task<IEnumerable<App.BLL.DTO.Task>> GetAllWithRelationsAsync(bool noTracking = true)
    {
        var dalItems = await ServiceRepository.GetAllWithRelationsAsync(noTracking);
        return dalItems.Select(item => Mapper.Map(item)!);
    }

    public async Task<App.BLL.DTO.Task?> GetWithRelationsAsync(Guid id, bool noTracking = true)
    {
        var dalItem = await ServiceRepository.GetWithRelationsAsync(id, noTracking);
        return Mapper.Map(dalItem);
    }

    public async Task<IEnumerable<App.BLL.DTO.Task>> GetByTaskKeeperAsync(Guid taskKeeperId, bool noTracking = true)
    {
        var dalItems = await ServiceRepository.GetByTaskKeeperAsync(taskKeeperId, noTracking);
        return dalItems.Select(item => Mapper.Map(item)!);
    }

    public async Task<IEnumerable<App.BLL.DTO.Task>> GetByDepartmentAsync(Guid departmentId, bool noTracking = true)
    {
        var dalItems = await ServiceRepository.GetByDepartmentAsync(departmentId, noTracking);
        return dalItems.Select(item => Mapper.Map(item)!);
    }

    public async Task<IEnumerable<App.BLL.DTO.Task>> GetByStatusAsync(App.BLL.DTO.TaskStatus status, bool noTracking = true)
    {
        var dalStatus = _convertBllToDalStatus(status);
        var dalItems = await ServiceRepository.GetByStatusAsync(dalStatus, noTracking);
        return dalItems.Select(item => Mapper.Map(item)!);
    }

    public async Task<IEnumerable<App.BLL.DTO.Task>> GetByPriorityAsync(int priority, bool noTracking = true)
    {
        var dalItems = await ServiceRepository.GetByPriorityAsync(priority, noTracking);
        return dalItems.Select(item => Mapper.Map(item)!);
    }

    public async Task<IEnumerable<App.BLL.DTO.Task>> GetOverdueTasksAsync(bool noTracking = true)
    {
        var dalItems = await ServiceRepository.GetOverdueTasksAsync(noTracking);
        return dalItems.Select(item => Mapper.Map(item)!);
    }

    public async Task<IEnumerable<App.BLL.DTO.Task>> GetTasksDueSoonAsync(int daysAhead = 7, bool noTracking = true)
    {
        var dalItems = await ServiceRepository.GetTasksDueSoonAsync(daysAhead, noTracking);
        return dalItems.Select(item => Mapper.Map(item)!);
    }

    public async Task<IEnumerable<App.BLL.DTO.Task>> GetByCreatorAsync(string createdBy, bool noTracking = true)
    {
        var dalItems = await ServiceRepository.GetByCreatorAsync(createdBy, noTracking);
        return dalItems.Select(item => Mapper.Map(item)!);
    }

    // Clean business logic methods
    public async Task<App.BLL.DTO.Task?> CompleteTaskAsync(Guid taskId, string? completionNotes = null)
    {
        var dalTask = await ServiceRepository.GetWithRelationsAsync(taskId, false);
        if (dalTask == null) return null;

        // Update task properties using BLL logic
        dalTask.TaskStatus = App.DAL.DTO.TaskStatus.Completed;
        dalTask.CompletedAt = DateTime.UtcNow;
        dalTask.CompletionNotes = completionNotes;

        // Simple repository update
        var updatedTask = ServiceRepository.Update(dalTask);
        await ServiceUOW.SaveChangesAsync();

        return Mapper.Map(updatedTask);
    }

    public async Task<App.BLL.DTO.Task?> AssignTaskAsync(Guid taskId, Guid taskKeeperId)
    {
        var dalTask = await ServiceRepository.GetWithRelationsAsync(taskId, false);
        if (dalTask == null) return null;

        // Update task properties using BLL logic
        dalTask.TaskKeeperId = taskKeeperId;
        
        // Business rule: If task was Open and now has an assignee, change to InProgress
        if (dalTask.TaskStatus == App.DAL.DTO.TaskStatus.Open)
        {
            dalTask.TaskStatus = App.DAL.DTO.TaskStatus.InProgress;
        }

        // Simple repository update
        var updatedTask = ServiceRepository.Update(dalTask);
        await ServiceUOW.SaveChangesAsync();

        return Mapper.Map(updatedTask);
    }

    public async Task<App.BLL.DTO.Task?> ChangeTaskStatusAsync(Guid taskId, App.BLL.DTO.TaskStatus newStatus)
    {
        var dalTask = await ServiceRepository.GetWithRelationsAsync(taskId, false);
        if (dalTask == null) return null;

        // Update task properties using BLL logic
        dalTask.TaskStatus = _convertBllToDalStatus(newStatus);
        
        // Business rule: If marking as completed, set completion time
        if (newStatus == App.BLL.DTO.TaskStatus.Completed && dalTask.CompletedAt == null)
        {
            dalTask.CompletedAt = DateTime.UtcNow;
        }

        // Simple repository update
        var updatedTask = ServiceRepository.Update(dalTask);
        await ServiceUOW.SaveChangesAsync();

        return Mapper.Map(updatedTask);
    }

    public async Task<App.BLL.DTO.Task?> UpdateTaskPriorityAsync(Guid taskId, int newPriority)
    {
        // Business validation
        if (newPriority < 1 || newPriority > 5) 
            throw new ArgumentException("Priority must be between 1 and 5");

        var dalTask = await ServiceRepository.GetWithRelationsAsync(taskId, false);
        if (dalTask == null) return null;

        // Update task properties using BLL logic
        dalTask.Priority = newPriority;

        // Simple repository update
        var updatedTask = ServiceRepository.Update(dalTask);
        await ServiceUOW.SaveChangesAsync();

        return Mapper.Map(updatedTask);
    }

    // Task statistics for specific filters
    public async Task<TaskStatistics> GetTaskStatisticsAsync()
    {
        var allTasks = await GetAllWithRelationsAsync(true);
        var taskList = allTasks.ToList();

        return new TaskStatistics
        {
            TotalTasks = taskList.Count,
            OpenTasks = taskList.Count(t => t.TaskStatus == App.BLL.DTO.TaskStatus.Open),
            InProgressTasks = taskList.Count(t => t.TaskStatus == App.BLL.DTO.TaskStatus.InProgress),
            CompletedTasks = taskList.Count(t => t.TaskStatus == App.BLL.DTO.TaskStatus.Completed),
            OverdueTasks = taskList.Count(t => t.DeadLine < DateTime.UtcNow && t.TaskStatus != App.BLL.DTO.TaskStatus.Completed),
            HighPriorityTasks = taskList.Count(t => t.Priority >= 4 && t.TaskStatus != App.BLL.DTO.TaskStatus.Completed),
            TasksDueSoon = taskList.Count(t => t.DeadLine <= DateTime.UtcNow.AddDays(7) && t.DeadLine >= DateTime.UtcNow && t.TaskStatus != App.BLL.DTO.TaskStatus.Completed),
            CompletionRate = taskList.Count > 0 ? (double)taskList.Count(t => t.TaskStatus == App.BLL.DTO.TaskStatus.Completed) / taskList.Count * 100 : 0
        };
    }

    private App.DAL.DTO.TaskStatus _convertBllToDalStatus(App.BLL.DTO.TaskStatus bllStatus)
    {
        return bllStatus switch
        {
            App.BLL.DTO.TaskStatus.Open => App.DAL.DTO.TaskStatus.Open,
            App.BLL.DTO.TaskStatus.InProgress => App.DAL.DTO.TaskStatus.InProgress,
            App.BLL.DTO.TaskStatus.Completed => App.DAL.DTO.TaskStatus.Completed,
            App.BLL.DTO.TaskStatus.Cancelled => App.DAL.DTO.TaskStatus.Cancelled,
            App.BLL.DTO.TaskStatus.OnHold => App.DAL.DTO.TaskStatus.OnHold,
            _ => throw new ArgumentException($"Unknown TaskStatus: {bllStatus}")
        };
    }
}