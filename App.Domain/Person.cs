using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Domain.Identity;
using Base.Domain;

namespace App.Domain;

public class Person : BaseEntityUser<AppUser>
{
    [MaxLength(128)]
    public string PersonName { get; set; } = default!;

    public ICollection<Contact>? Contacts { get; set; }
    
    public ICollection<DepartmentPerson>? DepartmentPersons { get; set; }

    [NotMapped]
    public IEnumerable<Department>? Departments => DepartmentPersons?.Select(x => x.Department).OfType<Department>();
    
    // Task relationships
    public ICollection<Task>? AssignedTasks { get; set; }
    
    // Task-related computed properties
    [NotMapped]
    public IEnumerable<Task>? OpenAssignedTasks => AssignedTasks?.Where(t => t.TaskStatus == TaskStatus.Open);
    
    [NotMapped]
    public IEnumerable<Task>? InProgressAssignedTasks => AssignedTasks?.Where(t => t.TaskStatus == TaskStatus.InProgress);
    
    [NotMapped]
    public IEnumerable<Task>? CompletedAssignedTasks => AssignedTasks?.Where(t => t.TaskStatus == TaskStatus.Completed);
    
    [NotMapped]
    public IEnumerable<Task>? OverdueAssignedTasks => AssignedTasks?.Where(t => 
        t.DeadLine < DateTime.UtcNow && t.TaskStatus != TaskStatus.Completed);
    
    [NotMapped]
    public IEnumerable<Task>? HighPriorityAssignedTasks => AssignedTasks?.Where(t => 
        t.Priority >= 4 && t.TaskStatus != TaskStatus.Completed);
    
    // Tasks from departments this person manages (if they are a manager)
    [NotMapped]
    public IEnumerable<Task>? ManagedDepartmentTasks => 
        Departments?.Where(d => d.ManagerId == Id)
            .SelectMany(d => d.Tasks ?? Enumerable.Empty<Task>());
    
    // Task statistics
    [NotMapped]
    public int AssignedTaskCount => AssignedTasks?.Count() ?? 0;
    
    [NotMapped]
    public int CompletedAssignedTaskCount => CompletedAssignedTasks?.Count() ?? 0;
    
    [NotMapped]
    public int OverdueAssignedTaskCount => OverdueAssignedTasks?.Count() ?? 0;
    
    [NotMapped]
    public double TaskCompletionRate => AssignedTaskCount > 0 ? (double)CompletedAssignedTaskCount / AssignedTaskCount * 100 : 0;
    
    [NotMapped]
    public int ManagedTaskCount => ManagedDepartmentTasks?.Count() ?? 0;
}