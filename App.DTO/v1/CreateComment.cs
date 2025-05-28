using System.ComponentModel.DataAnnotations;

namespace App.DTO.v1;

public class CreateComment
{
    [Required]
    [MaxLength(1000)]
    public string Content { get; set; } = default!;
    
    // Either PostId or FeedbackId must be provided, but not both
    public Guid? PostId { get; set; }
    public Guid? FeedbackId { get; set; }
}