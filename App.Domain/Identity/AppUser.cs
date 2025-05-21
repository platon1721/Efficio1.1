using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Base.Domain.Identity;

namespace App.Domain.Identity;

public class AppUser : BaseUser<AppUserRole>
{
    [MinLength(1)]
    [MaxLength(128)]
    public string FirstName { get; set; } = default!;
 
    [MinLength(1)]
    [MaxLength(128)]
    public string LastName { get; set; } = default!;

    public ICollection<Person>? Persons { get; set; }
    public ICollection<ContactType>? ContactTypes { get; set; }

    public ICollection<AppRefreshToken>? RefreshTokens { get; set; }
}