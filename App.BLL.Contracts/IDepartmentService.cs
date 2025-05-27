using App.BLL.DTO;
using Base.BLL.Contracts;

namespace App.BLL.Contracts;

public interface IDepartmentService : IBaseService<Department>
{
    // Existing methods
    Task<IEnumerable<Department>> GetAllWithManagerAsync(bool noTracking = true);
    Task<Department?> GetWithPersonsAsync(Guid id, bool noTracking = true);
    Task<Department?> GetWithTasksAsync(Guid id, bool noTracking = true);
    Task<Department?> GetWithAllRelationsAsync(Guid id, bool noTracking = true);
    Task<IEnumerable<Department>> GetAllWithTasksAsync(bool noTracking = true);
    
    // NEW METHODS FOR FEEDBACK SUPPORT
    Task<Department?> GetWithFeedbacksAsync(Guid id, bool noTracking = true);
    Task<IEnumerable<Department>> GetAllWithFeedbacksAsync(bool noTracking = true);
    
    // Business methods for managing department feedbacks
    Task<IEnumerable<Feedback>> GetDepartmentFeedbacksAsync(Guid departmentId, bool noTracking = true);
    Task<IEnumerable<App.BLL.DTO.Task>> GetDepartmentTasksAsync(Guid departmentId, bool noTracking = true);
    
    // Statistics methods
    Task<DepartmentStatistics> GetDepartmentStatisticsAsync(Guid departmentId);
}

// Statistics class for department analytics
public class DepartmentStatistics
{
    public int TotalPersons { get; set; }
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int OverdueTasks { get; set; }
    public int TotalFeedbacks { get; set; }
    public int TotalPosts { get; set; }
    public double TaskCompletionRate { get; set; }
    public DateTime? LastFeedbackDate { get; set; }
    public DateTime? LastTaskCompletionDate { get; set; }
}