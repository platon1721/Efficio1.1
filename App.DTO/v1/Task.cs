using System.ComponentModel.DataAnnotations;
using Base.Contracts;

namespace App.DTO.v1;

public class Task : IDomainId
{
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(120)]
    public string Title { get; set; } = default!;
    
    [Required]
    [MaxLength(5000)]
    public string Description { get; set; } = default!;
    
    [Range(1, 5)]
    public int Priority { get; set; } = 1;
    
    public DateTime DeadLine { get; set; }
    
    public TaskStatus TaskStatus { get; set; } = TaskStatus.Open;
    
    public Guid? TaskKeeperId { get; set; }
    public string? TaskKeeperName { get; set; } // For display purposes
    
    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; } // For display purposes
    
    public DateTime? CompletedAt { get; set; }
    
    [MaxLength(1000)]
    public string? CompletionNotes { get; set; }
}

public enum TaskStatus
{
    Open = 0,
    InProgress = 1,
    Completed = 2,
    Cancelled = 3,
    OnHold = 4
}