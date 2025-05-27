using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Base.Contracts;

namespace App.DAL.DTO;

public class Feedback : IDomainId
{
    public Guid Id { get; set; }
    
    [MaxLength(120)]
    public string Title { get; set; } = default!;
    
    [MaxLength(5000)]
    public string Description { get; set; } = default!;
    
    // // Picture
    // public string? ImagePath { get; set; }
    // [Range(0, 5 * 1024 * 1024, ErrorMessage = "Picture size must be between 0 and 5MB.")]
    // public long? ImageSize { get; set; }
    
    // Tags connection
    public ICollection<FeedbackTag>? FeedbackTags { get; set; }
    
    // Department connection (optional - feedback can be targeted to a specific department)
    public Guid? DepartmentId { get; set; }
    public Department? Department { get; set; }
    
    // Comments on this feedback
    public ICollection<Comment>? Comments { get; set; }
}