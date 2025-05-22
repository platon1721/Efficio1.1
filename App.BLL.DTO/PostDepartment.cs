using Base.Contracts;

namespace App.BLL.DTO;

public class PostDepartment : IDomainId
{
    public Guid Id { get; set; }
    
    public Guid PostId { get; set; }
    public Post? Post { get; set; }
    
    public Guid DepartmentId { get; set; }
    public Department? Department { get; set; }
}