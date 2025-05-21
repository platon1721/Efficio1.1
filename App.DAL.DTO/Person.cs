using System.ComponentModel.DataAnnotations;
using Base.Contracts;

namespace App.DAL.DTO;

public class Person: IDomainId
{
    public Guid Id { get; set; }

    [MaxLength(128)]
    public string PersonName { get; set; } = default!;

    public ICollection<Contact>? Contacts { get; set; }
}