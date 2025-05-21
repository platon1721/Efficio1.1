using System.ComponentModel.DataAnnotations;

namespace App.DTO.v1;

public class ContactTypeCreate
{
    [Required]
    [MaxLength(128, ErrorMessageResourceType = typeof(Base.Resources.Common), ErrorMessageResourceName = "MaxLength")]
    public string ContactTypeName { get; set; } = default!;
}