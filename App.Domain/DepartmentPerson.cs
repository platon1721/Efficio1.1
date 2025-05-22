using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace App.Domain;

public class DepartmentPerson : BaseEntity
{
    public Guid DepartmentId { get; set; }
    public Department? Department { get; set; }
    
    public Guid PersonId { get; set; }
    public Person? Person { get; set; }
    
    [MaxLength(128)]
    public string? Position { get; set; }
    
}