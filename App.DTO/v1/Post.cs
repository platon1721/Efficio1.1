using System.ComponentModel.DataAnnotations;
using Base.Contracts;

namespace App.DTO.v1;

public class Post : IDomainId
{
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(120)]
    public string Title { get; set; } = default!;
    
    [Required]
    [MaxLength(5000)]
    public string Description { get; set; } = default!;
}