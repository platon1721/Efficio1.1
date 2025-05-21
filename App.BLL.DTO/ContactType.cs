using System.ComponentModel.DataAnnotations;
using Base.Contracts;

namespace App.BLL.DTO;

public class ContactType: IDomainId
{
    public Guid Id { get; set; }

    [MaxLength(128, ErrorMessageResourceType = typeof(Base.Resources.Common), ErrorMessageResourceName = "MaxLength")]
    [Display(Name = nameof(ContactTypeName), Prompt = nameof(ContactTypeName), ResourceType = typeof(App.Resources.Domain.ContactType))]
    public string ContactTypeName { get; set; } = default!;

    [MaxLength(128, ErrorMessageResourceType = typeof(Base.Resources.Common), ErrorMessageResourceName = "MaxLength")]
    [Display(Name = nameof(Contacts), Prompt = nameof(Contacts), ResourceType = typeof(App.Resources.Domain.ContactType))]
    public ICollection<Contact>? Contacts { get; set; }

}