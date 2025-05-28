using Base.Contracts;

namespace App.DTO.v1;

public class PostDepartment : IDomainId
{
    public Guid Id { get; set; }
    
    public Guid PostId { get; set; }
    public Guid DepartmentId { get; set; }
    
    // For display purposes
    public string? PostTitle { get; set; }
    public string? DepartmentName { get; set; }
}