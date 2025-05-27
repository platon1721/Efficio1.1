using System.ComponentModel.DataAnnotations;
using Base.Contracts;

namespace App.BLL.DTO;

public class Post : IDomainId
{
    public Guid Id { get; set; }
    
    [MaxLength(120)]
    public string Title { get; set; } = default!;
    
    [MaxLength(5000)]
    public string Description { get; set; } = default!;
    
    // Navigation properties
    public ICollection<PostTag>? PostTags { get; set; }
    public ICollection<PostDepartment>? PostDepartments { get; set; }
    
    public ICollection<Comment>? Comments { get; set; }
    
    // Computed properties for easier access
    public ICollection<Tag>? Tags => PostTags?.Select(pt => pt.Tag).OfType<Tag>().ToList();
    public ICollection<Department>? Departments => PostDepartments?.Select(pd => pd.Department).OfType<Department>().ToList();
}