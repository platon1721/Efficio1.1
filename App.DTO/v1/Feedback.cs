using System.ComponentModel.DataAnnotations;
using Base.Contracts;

namespace App.DTO.v1;

public class Feedback : IDomainId
{
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(120)]
    public string Title { get; set; } = default!;
    
    [Required]
    [MaxLength(5000)]
    public string Description { get; set; } = default!;
    
    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; } // For display purposes
    
    // For displaying tags and comments count
    public ICollection<string>? TagTitles { get; set; }
    public int CommentCount { get; set; }
}