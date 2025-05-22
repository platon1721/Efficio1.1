using System.ComponentModel.DataAnnotations;
using Base.Contracts;

namespace App.DAL.DTO;

public class DepartmentPerson: IDomainId
{
    public Guid Id { get; set; }
    
    public Guid DepartmentId { get; set; }
    public Department? Department { get; set; }
    
    public Guid PersonId { get; set; }
    public Person? Person { get; set; }
    
    [MaxLength(128)]
    public string? Position { get; set; }
}