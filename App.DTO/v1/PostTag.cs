using Base.Contracts;

namespace App.DTO.v1;

public class PostTag : IDomainId
{
    public Guid Id { get; set; }
    
    public Guid PostId { get; set; }
    public Guid TagId { get; set; }
    
    // For display purposes
    public string? PostTitle { get; set; }
    public string? TagTitle { get; set; }
}