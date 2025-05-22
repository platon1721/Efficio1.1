using Base.Domain;

namespace App.Domain;

public class PostDepartment : BaseEntity
{
    public Guid PostId { get; set; }
    public Post? Post { get; set; }
    
    public Guid DepartmentId { get; set; }
    public Department? Department { get; set; }
}