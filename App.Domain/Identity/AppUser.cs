using System.Collections;
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
    
    public Person? Person { get; set; }

    // Base module
    public ICollection<Person>? Persons { get; set; }
    public ICollection<ContactType>? ContactTypes { get; set; }
    public ICollection<Department>? Departments { get; set; }
    public ICollection<DepartmentPerson>? DepartmentPersons { get; set; }
    
    // Communication module
    public ICollection<Post>? Posts { get; set; }
    public ICollection<Tag>? Tags { get; set; }
    public ICollection<PostTag>? PostTags { get; set; }
    public ICollection<PostDepartment>? PostDepartments { get; set; }
    public ICollection<Task>? Tasks { get; set; }
    public ICollection<Comment>? Comments { get; set; }
    public ICollection<Feedback>? Feedbacks { get; set; }
    public ICollection<FeedbackTag>? FeedbackTags { get; set; }

    public ICollection<AppRefreshToken>? RefreshTokens { get; set; }
}