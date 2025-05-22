using System.ComponentModel.DataAnnotations;
using Base.Contracts;

namespace App.BLL.DTO;

public class Person: IDomainId
{
    public Guid Id { get; set; }

    [MaxLength(128, ErrorMessageResourceType = typeof(Base.Resources.Common), ErrorMessageResourceName = "MaxLength")]
    [Display(Name = nameof(PersonName), Prompt = nameof(PersonName), ResourceType = typeof(App.Resources.Domain.Person))]
    public string PersonName { get; set; } = default!;
    

    [MaxLength(128, ErrorMessageResourceType = typeof(Base.Resources.Common), ErrorMessageResourceName = "MaxLength")]
    [Display(Name = nameof(Contacts), Prompt = nameof(Contacts), ResourceType = typeof(App.Resources.Domain.Person))]
    public ICollection<Contact>? Contacts { get; set; }
    
    // Add departments
    public ICollection<Department>? Departments { get; set; }
    
    // Optionally include DepartmentPersons if needed
    public ICollection<DepartmentPerson>? DepartmentPersons { get; set; }
}