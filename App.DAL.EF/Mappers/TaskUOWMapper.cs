using Base.Contracts;

namespace App.DAL.EF.Mappers;

public class TaskUOWMapper : IMapper<App.DAL.DTO.Task, App.Domain.Task>
{
    public App.DAL.DTO.Task? Map(App.Domain.Task? entity)
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
            TaskKeeper = entity.TaskKeeper == null
                ? null
                : new App.DAL.DTO.Person()
                {
                    Id = entity.TaskKeeper.Id,
                    PersonName = entity.TaskKeeper.PersonName,
                },
            DepartmentId = entity.DepartmentId,
            Department = entity.Department == null
                ? null
                : new App.DAL.DTO.Department()
                {
                    Id = entity.Department.Id,
                    DepartmentName = entity.Department.DepartmentName,
                },
            CompletedAt = entity.CompletedAt,
            CompletionNotes = entity.CompletionNotes
        };

        return res;
    }

    public App.Domain.Task? Map(App.DAL.DTO.Task? entity)
    {
        if (entity == null) return null;

        var res = new App.Domain.Task()
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            Priority = entity.Priority,
            DeadLine = entity.DeadLine,
            TaskStatus = (App.Domain.TaskStatus)entity.TaskStatus,
            TaskKeeperId = entity.TaskKeeperId,
            DepartmentId = entity.DepartmentId,
            CompletedAt = entity.CompletedAt,
            CompletionNotes = entity.CompletionNotes
        };

        return res;
    }
}