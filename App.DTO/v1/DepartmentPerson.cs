using System.ComponentModel.DataAnnotations;
using Base.Contracts;

namespace App.DTO.v1;

public class DepartmentPerson : IDomainId
{
    public Guid Id { get; set; }
    
    public Guid DepartmentId { get; set; }
    public Guid PersonId { get; set; }
    
    [MaxLength(128)]
    public string? Position { get; set; }
}   