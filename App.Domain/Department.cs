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
    public IEnumerable<Person>? Persons => DepartmentPersons.Select(p => p.Person).OfType<Person>();
    
    
    // Department Post
    public ICollection<PostDepartment>? PostDepartments { get; set; }
    [NotMapped]
    public IEnumerable<Post>? Posts => PostDepartments.Select(p => p.Post).OfType<Post>();
    
    // [Required]
    // public Guid HeadId { get; set; }
    // public User Head { get; set; } = default!;
    
    
    // public Guid? HeadDepartmentId { get; set; }
    // public Department? HeadDepartment { get; set; }
    // public ICollection<UserDepartment> UserDepartments { get; set; } = new List<UserDepartment>();
    // public ICollection<Department> SubDepartments { get; set; } = new List<Department>();
    // public ICollection<PostDepartment> PostDepartments{ get; set; } = new List<PostDepartment>();
    //
    // [NotMapped]
    // public ICollection<Post> Posts => PostDepartments.Select(x => x.Post).ToList();
    // [NotMapped]
    // public ICollection<User> Users => UserDepartments.Select(x => x.User).ToList();
}