// App.DTO/v1/Mappers/TaskMapper.cs
using Base.Contracts;

namespace App.DTO.v1.Mappers;

public class TaskMapper : IMapper<App.DTO.v1.Task, App.BLL.DTO.Task>
{
    public App.DTO.v1.Task? Map(App.BLL.DTO.Task? entity)
    {
        if (entity == null) return null;
        
        return new App.DTO.v1.Task
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            Priority = entity.Priority,
            DeadLine = entity.DeadLine,
            TaskStatus = (App.DTO.v1.TaskStatus)entity.TaskStatus,
            TaskKeeperId = entity.TaskKeeperId,
            TaskKeeperName = entity.TaskKeeper?.PersonName,
            DepartmentId = entity.DepartmentId,
            DepartmentName = entity.Department?.DepartmentName,
            CompletedAt = entity.CompletedAt,
            CompletionNotes = entity.CompletionNotes
        };
    }

    public App.BLL.DTO.Task? Map(App.DTO.v1.Task? entity)
    {
        if (entity == null) return null;
        
        return new App.BLL.DTO.Task
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            Priority = entity.Priority,
            DeadLine = entity.DeadLine,
            TaskStatus = (App.BLL.DTO.TaskStatus)entity.TaskStatus,
            TaskKeeperId = entity.TaskKeeperId,
            DepartmentId = entity.DepartmentId,
            CompletedAt = entity.CompletedAt,
            CompletionNotes = entity.CompletionNotes
        };
    }
    
    public App.BLL.DTO.Task Map(App.DTO.v1.CreateTask entity)
    {
        return new App.BLL.DTO.Task
        {
            Id = Guid.NewGuid(),
            Title = entity.Title,
            Description = entity.Description,
            Priority = entity.Priority,
            DeadLine = entity.DeadLine,
            TaskStatus = App.BLL.DTO.TaskStatus.Open,
            TaskKeeperId = entity.TaskKeeperId,
            DepartmentId = entity.DepartmentId
        };
    }
    
    public App.BLL.DTO.Task Map(App.DTO.v1.UpdateTask entity, Guid id)
    {
        return new App.BLL.DTO.Task
        {
            Id = id,
            Title = entity.Title,
            Description = entity.Description,
            Priority = entity.Priority,
            DeadLine = entity.DeadLine,
            TaskStatus = (App.BLL.DTO.TaskStatus)entity.TaskStatus,
            TaskKeeperId = entity.TaskKeeperId,
            DepartmentId = entity.DepartmentId,
            CompletionNotes = entity.CompletionNotes
        };
    }
}