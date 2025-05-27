using Base.Contracts;

namespace App.BLL.Mappers;

public class DepartmentBLLMapper : IMapper<App.BLL.DTO.Department, App.DAL.DTO.Department>
{
    public App.BLL.DTO.Department? Map(App.DAL.DTO.Department? entity)
    {
        if (entity == null) return null;
        
        var res = new App.BLL.DTO.Department()
        {
            Id = entity.Id,
            DepartmentName = entity.DepartmentName,
            
            ManagerId = entity.ManagerId,
            Manager = entity.Manager != null ? new App.BLL.DTO.Person()
            {
                Id = entity.Manager.Id,
                PersonName = entity.Manager.PersonName
            } : null,
            
            Persons = entity.Persons?.Select(p => new App.BLL.DTO.Person()
            {
                Id = p.Id,
                PersonName = p.PersonName
            }).ToList(),
            
            // Map Feedbacks
            Feedbacks = entity.Feedbacks?.Select(f => new App.BLL.DTO.Feedback()
            {
                Id = f.Id,
                Title = f.Title,
                Description = f.Description,
                DepartmentId = f.DepartmentId,
                FeedbackTags = f.FeedbackTags?.Select(ft => new App.BLL.DTO.FeedbackTag()
                {
                    Id = ft.Id,
                    FeedbackId = ft.FeedbackId,
                    TagId = ft.TagId,
                    Tag = ft.Tag != null ? new App.BLL.DTO.Tag()
                    {
                        Id = ft.Tag.Id,
                        Title = ft.Tag.Title,
                        Description = ft.Tag.Description
                    } : null
                }).ToList(),
                Comments = f.Comments?.Select(c => new App.BLL.DTO.Comment()
                {
                    Id = c.Id,
                    Content = c.Content,
                    FeedbackId = c.FeedbackId
                }).ToList()
            }).ToList(),
            
            // Map Tasks
            Tasks = entity.Tasks?.Select(t => new App.BLL.DTO.Task()
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Priority = t.Priority,
                DeadLine = t.DeadLine,
                TaskStatus = (App.BLL.DTO.TaskStatus)t.TaskStatus,
                TaskKeeperId = t.TaskKeeperId,
                DepartmentId = t.DepartmentId,
                CompletedAt = t.CompletedAt,
                CompletionNotes = t.CompletionNotes
            }).ToList(),
            
            // Map PostDepartments
            PostDepartments = entity.PostDepartments?.Select(pd => new App.BLL.DTO.PostDepartment()
            {
                Id = pd.Id,
                PostId = pd.PostId,
                DepartmentId = pd.DepartmentId,
                Post = pd.Post != null ? new App.BLL.DTO.Post()
                {
                    Id = pd.Post.Id,
                    Title = pd.Post.Title,
                    Description = pd.Post.Description
                } : null
            }).ToList()
        };
        
        return res;
    }

    public App.DAL.DTO.Department? Map(App.BLL.DTO.Department? entity)
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
            
            Persons = entity.Persons?.Select(p => new App.DAL.DTO.Person()
            {
                Id = p.Id,
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
                    TagId = ft.TagId
                }).ToList(),
                Comments = f.Comments?.Select(c => new App.DAL.DTO.Comment()
                {
                    Id = c.Id,
                    Content = c.Content,
                    FeedbackId = c.FeedbackId
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
                DepartmentId = t.DepartmentId,
                CompletedAt = t.CompletedAt,
                CompletionNotes = t.CompletionNotes
            }).ToList(),
            
            // Map PostDepartments
            PostDepartments = entity.PostDepartments?.Select(pd => new App.DAL.DTO.PostDepartment()
            {
                Id = pd.Id,
                PostId = pd.PostId,
                DepartmentId = pd.DepartmentId
            }).ToList()
        };
        
        return res;
    }
}