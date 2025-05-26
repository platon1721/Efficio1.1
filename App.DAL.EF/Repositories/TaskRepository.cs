using App.DAL.Contracts;
using App.DAL.EF.Mappers;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories;

public class TaskRepository : BaseRepository<App.DAL.DTO.Task, App.Domain.Task>, ITaskRepository
{
    public TaskRepository(AppDbContext repositoryDbContext) : base(repositoryDbContext, new TaskUOWMapper())
    {
    }

    public async Task<IEnumerable<App.DAL.DTO.Task>> GetAllWithRelationsAsync(bool noTracking = true)
    {
        var query = RepositoryDbSet.AsQueryable();
        
        if (noTracking)
        {
            query = query.AsNoTracking();
        }
        
        var domainTasks = await query
            .Include(t => t.TaskKeeper)
            .Include(t => t.Department)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
            
        return domainTasks.Select(t => Mapper.Map(t)).OfType<App.DAL.DTO.Task>();
    }
    
    public async Task<App.DAL.DTO.Task?> GetWithRelationsAsync(Guid id, bool noTracking = true)
    {
        var query = RepositoryDbSet.AsQueryable();
        
        if (noTracking)
        {
            query = query.AsNoTracking();
        }
        
        var domainTask = await query
            .Include(t => t.TaskKeeper)
            .Include(t => t.Department)
            .FirstOrDefaultAsync(t => t.Id.Equals(id));
            
        return Mapper.Map(domainTask);
    }
    
    public async Task<IEnumerable<App.DAL.DTO.Task>> GetByTaskKeeperAsync(Guid taskKeeperId, bool noTracking = true)
    {
        var query = RepositoryDbSet.AsQueryable();
        
        if (noTracking)
        {
            query = query.AsNoTracking();
        }
        
        var domainTasks = await query
            .Where(t => t.TaskKeeperId == taskKeeperId)
            .Include(t => t.TaskKeeper)
            .Include(t => t.Department)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
            
        return domainTasks.Select(t => Mapper.Map(t)).OfType<App.DAL.DTO.Task>();
    }
    
    public async Task<IEnumerable<App.DAL.DTO.Task>> GetByDepartmentAsync(Guid departmentId, bool noTracking = true)
    {
        var query = RepositoryDbSet.AsQueryable();
        
        if (noTracking)
        {
            query = query.AsNoTracking();
        }
        
        var domainTasks = await query
            .Where(t => t.DepartmentId == departmentId)
            .Include(t => t.TaskKeeper)
            .Include(t => t.Department)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
            
        return domainTasks.Select(t => Mapper.Map(t)).OfType<App.DAL.DTO.Task>();
    }
    
    public async Task<IEnumerable<App.DAL.DTO.Task>> GetByStatusAsync(App.DAL.DTO.TaskStatus status, bool noTracking = true)
    {
        var query = RepositoryDbSet.AsQueryable();
        
        if (noTracking)
        {
            query = query.AsNoTracking();
        }
        
        var domainTasks = await query
            .Where(t => t.TaskStatus == (App.Domain.TaskStatus)status)
            .Include(t => t.TaskKeeper)
            .Include(t => t.Department)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
            
        return domainTasks.Select(t => Mapper.Map(t)).OfType<App.DAL.DTO.Task>();
    }
    
    public async Task<IEnumerable<App.DAL.DTO.Task>> GetByPriorityAsync(int priority, bool noTracking = true)
    {
        var query = RepositoryDbSet.AsQueryable();
        
        if (noTracking)
        {
            query = query.AsNoTracking();
        }
        
        var domainTasks = await query
            .Where(t => t.Priority == priority)
            .Include(t => t.TaskKeeper)
            .Include(t => t.Department)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
            
        return domainTasks.Select(t => Mapper.Map(t)).OfType<App.DAL.DTO.Task>();
    }
    
    public async Task<IEnumerable<App.DAL.DTO.Task>> GetOverdueTasksAsync(bool noTracking = true)
    {
        var query = RepositoryDbSet.AsQueryable();
        
        if (noTracking)
        {
            query = query.AsNoTracking();
        }
        
        var now = DateTime.UtcNow;
        var domainTasks = await query
            .Where(t => t.DeadLine < now && t.TaskStatus != App.Domain.TaskStatus.Completed)
            .Include(t => t.TaskKeeper)
            .Include(t => t.Department)
            .OrderBy(t => t.DeadLine)
            .ToListAsync();
            
        return domainTasks.Select(t => Mapper.Map(t)).OfType<App.DAL.DTO.Task>();
    }
    
    public async Task<IEnumerable<App.DAL.DTO.Task>> GetTasksDueSoonAsync(int daysAhead = 7, bool noTracking = true)
    {
        var query = RepositoryDbSet.AsQueryable();
        
        if (noTracking)
        {
            query = query.AsNoTracking();
        }
        
        var now = DateTime.UtcNow;
        var futureDate = now.AddDays(daysAhead);
        
        var domainTasks = await query
            .Where(t => t.DeadLine >= now && t.DeadLine <= futureDate && t.TaskStatus != App.Domain.TaskStatus.Completed)
            .Include(t => t.TaskKeeper)
            .Include(t => t.Department)
            .OrderBy(t => t.DeadLine)
            .ToListAsync();
            
        return domainTasks.Select(t => Mapper.Map(t)).OfType<App.DAL.DTO.Task>();
    }
    
    public async Task<IEnumerable<App.DAL.DTO.Task>> GetByCreatorAsync(string createdBy, bool noTracking = true)
    {
        var query = RepositoryDbSet.AsQueryable();
        
        if (noTracking)
        {
            query = query.AsNoTracking();
        }
        
        var domainTasks = await query
            .Where(t => t.CreatedBy == createdBy)
            .Include(t => t.TaskKeeper)
            .Include(t => t.Department)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
            
        return domainTasks.Select(t => Mapper.Map(t)).OfType<App.DAL.DTO.Task>();
    }
}