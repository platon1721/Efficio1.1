using System.ComponentModel.DataAnnotations;

namespace App.DTO.v1;

public class CreateDepartmentPerson
{
    public Guid DepartmentId { get; set; }
    public Guid PersonId { get; set; }
    
    [MaxLength(128)]
    public string? Position { get; set; }
}