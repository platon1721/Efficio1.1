using Microsoft.AspNetCore.Mvc.Filters;
using App.Domain;

namespace WebApp.Filters;

public class InitializeCollectionsFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        foreach (var parameter in context.ActionArguments.Values)
        {
            // Initialize collections enne validation'i
            switch (parameter)
            {
                case Department dept:
                    dept.DepartmentPersons ??= new List<DepartmentPerson>();
                    dept.PostDepartments ??= new List<PostDepartment>();
                    break;
                    
                case Person person:
                    person.Contacts ??= new List<Contact>();
                    person.DepartmentPersons ??= new List<DepartmentPerson>();
                    break;
                    
                case Post post:
                    post.PostTags ??= new List<PostTag>();
                    post.PostDepartments ??= new List<PostDepartment>();
                    break;
            }
        }
        
        base.OnActionExecuting(context);
    }
}