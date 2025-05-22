using System.ComponentModel.DataAnnotations;
using Base.Contracts;

namespace App.DTO.v1;

public class Department : IDomainId
{
    public Guid Id { get; set; }
    
    [Required]
    [StringLength(128, ErrorMessageResourceType = typeof(Base.Resources.Common), ErrorMessageResourceName = "MaxLength", MinimumLength = 1)]
    public string DepartmentName { get; set; } = default!;
    
    public Guid? ManagerId { get; set; }
}