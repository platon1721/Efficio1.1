using System.ComponentModel.DataAnnotations;

namespace App.DTO.v1;

public class CreateTask
{
    [Required]
    [MaxLength(120)]
    public string Title { get; set; } = default!;
    
    [Required] 
    [MaxLength(5000)]
    public string Description { get; set; } = default!;
    
    [Range(1, 5)]
    public int Priority { get; set; } = 1;
    
    public DateTime DeadLine { get; set; }
    
    public Guid? TaskKeeperId { get; set; }
    
    public Guid? DepartmentId { get; set; }
}