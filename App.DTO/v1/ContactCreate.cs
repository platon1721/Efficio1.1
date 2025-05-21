using System.ComponentModel.DataAnnotations;
using Base.Contracts;

namespace App.DTO.v1;

public class ContactCreate
{
    [Required]
    [MaxLength(128, ErrorMessageResourceType = typeof(Base.Resources.Common), ErrorMessageResourceName = "MaxLength")]
    public string Value { get; set; } = default!;
    
    public Guid ContactTypeId { get; set; }

    public Guid PersonId { get; set; }
}