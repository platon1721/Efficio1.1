using System.ComponentModel.DataAnnotations;

namespace App.DTO.v1;

public class CreateFeedback
{
    [Required]
    [MaxLength(120)]
    public string Title { get; set; } = default!;
    
    [Required]
    [MaxLength(5000)]
    public string Description { get; set; } = default!;
    
    public Guid? DepartmentId { get; set; }
    
    // Tags to be associated with this feedback
    public ICollection<Guid>? TagIds { get; set; }
}