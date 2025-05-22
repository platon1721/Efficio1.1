using System.ComponentModel.DataAnnotations;
using Base.Contracts;

namespace App.BLL.DTO;

public class Department : IDomainId
{
    public Guid Id { get; set; }
    
    [MaxLength(128)]
    public string DepartmentName { get; set; } = default!;
    
    public Guid? ManagerId { get; set; }
    public Person? Manager { get; set; }
    
    public ICollection<Person>? Persons { get; set; }
}