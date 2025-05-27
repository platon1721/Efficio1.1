using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Base.Domain;

namespace App.Domain;

public class Feedback: BaseEntity
{
    [MaxLength(120)]
    public string Title { get; set; } = default!;
    
    [MaxLength(5000)]
    public string Description { get; set; } = default!;
    
    
    // // Picture
    // public string? ImagePath { get; set; }
    // [Range(0, 5 * 1024 * 1024, ErrorMessage = "Picture size must be between 0 and 5MB.")]
    // public long? ImageSize { get; set; }
    
    // Tags
    public ICollection<FeedbackTag>? FeedbackTags { get; set; } = new List<FeedbackTag>();
    [NotMapped]
    public IEnumerable<Tag>? Tags => FeedbackTags?.Select(ft => ft.Tag).OfType<Tag>();
    
    
    // Department can be added.
    public Guid? DepartmentId { get; set; }
    public Department? Department { get; set; } = default!;
    
    // Comments on this feedback
    public ICollection<Comment>? Comments { get; set; } = new List<Comment>();
}