using Base.Contracts;

namespace App.DAL.DTO;

public class FeedbackTag : IDomainId
{
    public Guid Id { get; set; }
    
    public Guid FeedbackId { get; set; }
    public Feedback? Feedback { get; set; }
    
    public Guid TagId { get; set; }
    public Tag? Tag { get; set; }
}