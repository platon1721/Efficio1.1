using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Domain.Identity;
using Base.Domain;

namespace App.Domain;

public class Person : BaseEntityUser<AppUser>
{
    [MaxLength(128)]
    public string PersonName { get; set; } = default!;

    public ICollection<Contact>? Contacts { get; set; }
    
    public ICollection<DepartmentPerson>? DepartmentPersons { get; set; }
    [NotMapped]
    public ICollection<Department>? Departments => DepartmentPersons?.Select(x => x.Department).ToList();
}