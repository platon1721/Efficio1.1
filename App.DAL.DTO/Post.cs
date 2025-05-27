using System.ComponentModel.DataAnnotations;
using Base.Contracts;

namespace App.DAL.DTO;

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
    // Comments on this post
    public ICollection<Comment>? Comments { get; set; }
}