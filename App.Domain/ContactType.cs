using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using App.Domain.Identity;
using Base.Domain;

namespace App.Domain;

public class ContactType : BaseEntityUser<AppUser>
{
    [Column(TypeName = "jsonb")]
    public LangStr ContactTypeName { get; set; } = default!;

    public ICollection<Contact>? Contacts { get; set; }
}