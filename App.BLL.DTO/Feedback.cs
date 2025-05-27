using System.ComponentModel.DataAnnotations;
using Base.Contracts;

namespace App.BLL.DTO;

public class Feedback : IDomainId
{
    public Guid Id { get; set; }
    
    [MaxLength(120)]
    public string Title { get; set; } = default!;
    
    [MaxLength(5000)]
    public string Description { get; set; } = default!;
    
    // Tags connection
    public ICollection<FeedbackTag>? FeedbackTags { get; set; }
    
    // Department connection (optional - feedback can be targeted to a specific department)
    public Guid? DepartmentId { get; set; }
    public Department? Department { get; set; }
    
    // Comments on this feedback
    public ICollection<Comment>? Comments { get; set; }
    
    // Computed properties for easier access
    public ICollection<Tag>? Tags => FeedbackTags?.Select(ft => ft.Tag).OfType<Tag>().ToList();
}