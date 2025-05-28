using System.ComponentModel.DataAnnotations;
using Base.Contracts;

namespace App.DTO.v1;

public class Comment : IDomainId
{
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(1000)]
    public string Content { get; set; } = default!;
    
    // Comment can be on either a Post or Feedback (but not both)
    public Guid? PostId { get; set; }
    public string? PostTitle { get; set; }
    
    public Guid? FeedbackId { get; set; }
    public string? FeedbackTitle { get; set; }
    
    // Metadata for display
    public DateTime CreatedAt { get; set; }
    public string? AuthorName { get; set; }
}