using System.ComponentModel.DataAnnotations;
using Base.Contracts;

namespace App.DAL.DTO;

public class Department : IDomainId
{
    public Guid Id { get; set; }
    
    [MaxLength(128)]
    public string DepartmentName { get; set; } = default!;
    
    public Guid? ManagerId { get; set; }
    public Person? Manager { get; set; }
    
    public ICollection<Person>? Persons { get; set; }
    
    // Department Feedbacks (direct relationship)
    public ICollection<Feedback>? Feedbacks { get; set; }
    
    // Department Tasks (direct relationship)
    public ICollection<Task>? Tasks { get; set; }
    
    public ICollection<PostDepartment>? PostDepartments { get; set; }

}