using System.Collections;
using Base.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace App.Domain;

public class Department : BaseEntity
{
    [MaxLength(128)]
    // [Display(Name = nameof(DepartmentName), Prompt = nameof(DepartmentName), ResourceType = typeof(App.Resources.Domain.Department))]
    public string DepartmentName { get; set; } = default!;
    
    // Department Manager/Head (one Person is the head)
    // [Display(Name = nameof(Manager), Prompt = nameof(Manager), ResourceType = typeof(App.Resources.Domain.Department))]
    public Guid? ManagerId { get; set; }
    
    // [Display(Name = nameof(Manager), Prompt = nameof(Manager), ResourceType = typeof(App.Resources.Domain.Department))]
    public Person? Manager { get; set; }
    
    // Department Workers (collection of Persons)
    public ICollection<DepartmentPerson>? DepartmentPersons { get; set; }

    [NotMapped]
    public IEnumerable<Person>? Persons => DepartmentPersons?.Select(p => p.Person).OfType<Person>();
    
    
    // Department Post
    public ICollection<PostDepartment>? PostDepartments { get; set; }
    [NotMapped]
    public IEnumerable<Post>? Posts => PostDepartments?.Select(p => p.Post).OfType<Post>();
    
    
    // Department Tasks
    public ICollection<Task>? Tasks { get; set; }
    
    // Task-related computed properties
    [NotMapped]
    public IEnumerable<Task>? OpenTasks => Tasks?.Where(t => t.TaskStatus == TaskStatus.Open);
    
    [NotMapped]
    public IEnumerable<Task>? InProgressTasks => Tasks?.Where(t => t.TaskStatus == TaskStatus.InProgress);
    
    [NotMapped]
    public IEnumerable<Task>? CompletedTasks => Tasks?.Where(t => t.TaskStatus == TaskStatus.Completed);
    
    [NotMapped]
    public IEnumerable<Task>? OverdueTasks => Tasks?.Where(t => 
        t.DeadLine < DateTime.UtcNow && t.TaskStatus != TaskStatus.Completed);
    
    [NotMapped]
    public IEnumerable<Task>? HighPriorityTasks => Tasks?.Where(t => 
        t.Priority >= 4 && t.TaskStatus != TaskStatus.Completed);
    
    [NotMapped]
    public IEnumerable<Task>? TasksDueSoon => Tasks?.Where(t => 
        t.DeadLine <= DateTime.UtcNow.AddDays(7) && 
        t.DeadLine >= DateTime.UtcNow && 
        t.TaskStatus != TaskStatus.Completed);
    
    // Task statistics
    [NotMapped]
    public int TaskCount => Tasks?.Count() ?? 0;
    
    [NotMapped]
    public int CompletedTaskCount => CompletedTasks?.Count() ?? 0;
    
    [NotMapped]
    public int OpenTaskCount => OpenTasks?.Count() ?? 0;
    
    [NotMapped]
    public int OverdueTaskCount => OverdueTasks?.Count() ?? 0;
    
    [NotMapped]
    public double TaskCompletionRate => TaskCount > 0 ? (double)CompletedTaskCount / TaskCount * 100 : 0;
}