using System.ComponentModel.DataAnnotations;

namespace App.DTO.v1;

public class CreateTag
{
    [Required]
    [MaxLength(20)]
    public string Title { get; set; } = default!;
    
    [Required]
    [MaxLength(250)]
    public string Description { get; set; } = default!;
}