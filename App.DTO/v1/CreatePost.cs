using System.ComponentModel.DataAnnotations;

namespace App.DTO.v1;

public class CreatePost
{
    [Required]
    [MaxLength(120)]
    public string Title { get; set; } = default!;
    
    [Required]
    [MaxLength(5000)]
    public string Description { get; set; } = default!;
}