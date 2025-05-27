using Base.Domain;

namespace App.Domain;

public class FeedbackTag: BaseEntity
{
    public Guid FeedbackId { get; set; }
    public Feedback? Feedback { get; set; }
    
    public Guid TagId { get; set; }
    public Tag? Tag { get; set; }
}