using System.ComponentModel.DataAnnotations;

namespace App.DTO.v1.Identity;

public class Register
{
    [MaxLength(128)]
    public string Email { get; set; } = default!;

    [MaxLength(128)]
    public string Password { get; set; } = default!;

    [MinLength(1)]
    [MaxLength(128)]
    public string FirstName { get; set; }= default!;
        
    [MinLength(1)]
    [MaxLength(128)]
    public string LastName { get; set; }= default!;
}