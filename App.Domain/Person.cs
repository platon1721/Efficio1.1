using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using App.Domain.Identity;
using Base.Domain;

namespace App.Domain;

public class Person : BaseEntityUser<AppUser>
{
    [MaxLength(128)]
    public string PersonName { get; set; } = default!;

    public ICollection<Contact>? Contacts { get; set; }
}