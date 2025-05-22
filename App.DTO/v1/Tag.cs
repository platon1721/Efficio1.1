using System.ComponentModel.DataAnnotations;
using Base.Contracts;

namespace App.DTO.v1;

public class Tag : IDomainId
{ 
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string Title { get; set; } = default!;
    
    [Required]
    [MaxLength(250)]
    public string Description { get; set; } = default!;

   
}