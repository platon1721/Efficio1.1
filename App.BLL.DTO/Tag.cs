using System.ComponentModel.DataAnnotations;
using Base.Contracts;

namespace App.BLL.DTO;

public class Tag: IDomainId
{
    public Guid Id { get; set; }
    
    [MaxLength(20)]
    public string Title { get; set; } = default!;
    
    [MaxLength(250)]
    public string Description { get; set; } = default!;
}