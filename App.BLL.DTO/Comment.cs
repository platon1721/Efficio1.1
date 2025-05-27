using System.ComponentModel.DataAnnotations;
using Base.Contracts;

namespace App.BLL.DTO;

public class Comment : IDomainId
{
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(1000)]
    public string Content { get; set; } = default!;
    
    // Comment can be on either a Post or Feedback (but not both)
    public Guid? PostId { get; set; }
    public Post? Post { get; set; }
    
    public Guid? FeedbackId { get; set; }
    public Feedback? Feedback { get; set; }
    
    // Future: Nested/threaded comments support
    // public Guid? ParentCommentId { get; set; }
    // public Comment? ParentComment { get; set; }
    // public ICollection<Comment>? Replies { get; set; }
}