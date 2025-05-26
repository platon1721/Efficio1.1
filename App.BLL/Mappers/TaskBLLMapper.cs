using Base.Contracts;

namespace App.BLL.Mappers;

public class TaskBLLMapper : IMapper<App.BLL.DTO.Task, App.DAL.DTO.Task>
{
    public App.BLL.DTO.Task? Map(App.DAL.DTO.Task? entity)
    {
        if (entity == null) return null;
        
        var res = new App.BLL.DTO.Task()
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            Priority = entity.Priority,
            DeadLine = entity.DeadLine,
            TaskStatus = (App.BLL.DTO.TaskStatus)entity.TaskStatus,
            TaskKeeperId = entity.TaskKeeperId,
            TaskKeeper = entity.TaskKeeper == null ? null : new App.BLL.DTO.Person()
            {
                Id = entity.TaskKeeper.Id,
                PersonName = entity.TaskKeeper.PersonName
            },
            DepartmentId = entity.DepartmentId,
            Department = entity.Department == null ? null : new App.BLL.DTO.Department()
            {
                Id = entity.Department.Id,
                DepartmentName = entity.Department.DepartmentName
            },
            CompletedAt = entity.CompletedAt,
            CompletionNotes = entity.CompletionNotes
        };
        
        return res;
    }

    public App.DAL.DTO.Task? Map(App.BLL.DTO.Task? entity)
    {
        if (entity == null) return null;
        
        var res = new App.DAL.DTO.Task()
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            Priority = entity.Priority,
            DeadLine = entity.DeadLine,
            TaskStatus = (App.DAL.DTO.TaskStatus)entity.TaskStatus,
            TaskKeeperId = entity.TaskKeeperId,
            DepartmentId = entity.DepartmentId,
            CompletedAt = entity.CompletedAt,
            CompletionNotes = entity.CompletionNotes
        };
        
        return res;
    }
}