using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace App.Domain;

public class Tag : BaseEntity
{
    [MaxLength(20)]
    public string Title { get; set; } = default!;
    
    [MaxLength(250)]
    public string Description { get; set; } = default!;
}