using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace App.Domain;

public class Comment: BaseEntity
{
    [Required]
    [MaxLength(1000)]
    public string Content { get; set; } = default!;
    
    // Comment can be on either a Post or Feedback (but not both)
    public Guid? PostId { get; set; }
    public Post? Post { get; set; }
    
    public Guid? FeedbackId { get; set; }
    public Feedback? Feedback { get; set; }
    
    
    // Parent comment for nested/threaded comments (in future)
    // public Guid? ParentCommentId { get; set; }
    // public Comment? ParentComment { get; set; }
    // public ICollection<Comment>? Replies { get; set; } = new List<Comment>()
}