using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace App.Domain;

public class Task: BaseEntity
{
    
    [MaxLength(120)]
    public string Title { get; set; } = default!;
    
    [MaxLength(5000)]
    public string Description { get; set; } = default!;
    
    [Range(1, 5, ErrorMessage = "Priority must be between 1 (Low) and 5 (Critical)")]
    public int Priority { get; set; } = 1;
    
    public DateTime DeadLine { get; set; }
    
    public TaskStatus TaskStatus = TaskStatus.Open;
    
    // Task keeper (assigned person)
    public Guid? TaskKeeperId { get; set; }
    public Person? TaskKeeper { get; set; }
    
    // Department assignment (optional)
    public Guid? DepartmentId { get; set; }
    public Department? Department { get; set; }
    
    // Completion tracking
    public DateTime? CompletedAt { get; set; }
    
    [MaxLength(1000)]
    public string? CompletionNotes { get; set; }
}

public enum TaskStatus
{
    Open = 0,
    InProgress = 1,
    Completed = 2,
    Canceled = 3,
    OnHold = 4,
}