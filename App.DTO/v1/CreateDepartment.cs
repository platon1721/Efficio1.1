using System.ComponentModel.DataAnnotations;

namespace App.DTO.v1;

public class CreateDepartment
{
    [Required]
    [StringLength(128, ErrorMessageResourceType = typeof(Base.Resources.Common), ErrorMessageResourceName = "MaxLength", MinimumLength = 1)]
    public string DepartmentName { get; set; } = default!;
    
    public Guid? ManagerId { get; set; }
}