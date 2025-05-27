// App.DAL.EF/Mappers/DepartmentUOWMapper.cs - Updated
using Base.Contracts;

namespace App.DAL.EF.Mappers;

public class DepartmentUOWMapper : IMapper<App.DAL.DTO.Department, App.Domain.Department>
{
    public App.DAL.DTO.Department? Map(App.Domain.Department? entity)
    {
        if (entity == null) return null;
        
        var res = new App.DAL.DTO.Department()
        {
            Id = entity.Id,
            DepartmentName = entity.DepartmentName,
            
            ManagerId = entity.ManagerId,
            Manager = entity.Manager != null ? new App.DAL.DTO.Person()
            {
                Id = entity.Manager.Id,
                PersonName = entity.Manager.PersonName
            } : null,
            
            // Map persons from DepartmentPersons
            Persons = entity.DepartmentPersons?.Select(dp => dp.Person)
                .Where(p => p != null)
                .Select(p => new App.DAL.DTO.Person()
                {
                    Id = p!.Id,
                    PersonName = p.PersonName
                }).ToList(),
            
            // Map Feedbacks
            Feedbacks = entity.Feedbacks?.Select(f => new App.DAL.DTO.Feedback()
            {
                Id = f.Id,
                Title = f.Title,
                Description = f.Description,
                DepartmentId = f.DepartmentId,
                FeedbackTags = f.FeedbackTags?.Select(ft => new App.DAL.DTO.FeedbackTag()
                {
                    Id = ft.Id,
                    FeedbackId = ft.FeedbackId,
                    TagId = ft.TagId,
                    Tag = ft.Tag != null ? new App.DAL.DTO.Tag()
                    {
                        Id = ft.Tag.Id,
                        Title = ft.Tag.Title,
                        Description = ft.Tag.Description
                    } : null
                }).ToList()
            }).ToList(),
            
            // Map Tasks
            Tasks = entity.Tasks?.Select(t => new App.DAL.DTO.Task()
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Priority = t.Priority,
                DeadLine = t.DeadLine,
                TaskStatus = (App.DAL.DTO.TaskStatus)t.TaskStatus,
                TaskKeeperId = t.TaskKeeperId,
                DepartmentId = t.DepartmentId
            }).ToList(),
            
            // Map PostDepartments
            PostDepartments = entity.PostDepartments?.Select(pd => new App.DAL.DTO.PostDepartment()
            {
                Id = pd.Id,
                PostId = pd.PostId,
                DepartmentId = pd.DepartmentId,
                Post = pd.Post != null ? new App.DAL.DTO.Post()
                {
                    Id = pd.Post.Id,
                    Title = pd.Post.Title,
                    Description = pd.Post.Description
                } : null
            }).ToList()
        };
        
        return res;
    }

    public App.Domain.Department? Map(App.DAL.DTO.Department? entity)
    {
        if (entity == null) return null;
        
        var res = new App.Domain.Department()
        {
            Id = entity.Id,
            DepartmentName = entity.DepartmentName,
            
            ManagerId = entity.ManagerId,
            Manager = entity.Manager != null ? new App.Domain.Person()
            {
                Id = entity.Manager.Id,
                PersonName = entity.Manager.PersonName
            } : null,
            
            // We don't map complex collections back to domain in the mapper
            // These will be handled separately in the repository layer
        };
        
        return res;
    }
}