using Base.Contracts;

namespace App.DTO.v1;

public class FeedbackTag : IDomainId
{
    public Guid Id { get; set; }
    
    public Guid FeedbackId { get; set; }
    public Guid TagId { get; set; }
    
    // For display purposes
    public string? FeedbackTitle { get; set; }
    public string? TagTitle { get; set; }
}