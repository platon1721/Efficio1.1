using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Base.Domain;

namespace App.Domain;

public class Post : BaseEntity
{
    [MaxLength(120)]
    public string Title { get; set; } = default!;
    
    [MaxLength(5000)]
    public string Description { get; set; } = default!;
    
    // // Picture
    // public string? ImagePath { get; set; }
    // [Range(0, 5 * 1024 * 1024, ErrorMessage = "Picture size must be between 0 and 5MB.")]
    // public long? ImageSize { get; set; }
    
    // Tag connection
    public ICollection<PostTag>? PostTags { get; set; } = new List<PostTag>();
    
    [NotMapped]
    public IEnumerable<Tag>? Tags => PostTags?.Select(pt => pt.Tag).OfType<Tag>();
    
    // Comments added to the post
    public ICollection<Comment>? Comments { get; set; } = new List<Comment>();
    
    // Department connection
    public ICollection<PostDepartment>? PostDepartments { get; set; } = new List<PostDepartment>();
    [NotMapped]
    public IEnumerable<Department>? Departments => PostDepartments?.Select(pt => pt.Department).OfType<Department>();
}