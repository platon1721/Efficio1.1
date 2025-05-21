using Base.Contracts;

namespace App.DAL.DTO;

public class ContactType: IDomainId
{
    public Guid Id { get; set; }

    public string ContactTypeName { get; set; } = default!;

    public ICollection<Contact>? Contacts { get; set; }

}