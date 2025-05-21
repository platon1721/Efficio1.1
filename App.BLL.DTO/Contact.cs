using System.ComponentModel.DataAnnotations;
using Base.Contracts;

namespace App.BLL.DTO;

public class Contact : IDomainId
{
    public Guid Id { get; set; }

    
    [MaxLength(128, ErrorMessageResourceType = typeof(Base.Resources.Common), ErrorMessageResourceName = "MaxLength")]
    [Display(Name = nameof(Value), Prompt = nameof(Value), ResourceType = typeof(App.Resources.Domain.Contact))]
    public string Value { get; set; } = default!;
    
    
    [Display(Name = nameof(ContactType), Prompt = nameof(ContactType), ResourceType = typeof(App.Resources.Domain.Contact))]
    public Guid ContactTypeId { get; set; }
    
    [Display(Name = nameof(ContactType), Prompt = nameof(ContactType), ResourceType = typeof(App.Resources.Domain.Contact))]
    public ContactType? ContactType { get; set; }
    
    
    [Display(Name = nameof(Person), Prompt = nameof(Person), ResourceType = typeof(App.Resources.Domain.Contact))]
    public Guid PersonId { get; set; }
    [Display(Name = nameof(Person), Prompt = nameof(Person), ResourceType = typeof(App.Resources.Domain.Contact))]
    public Person? Person { get; set; }

}