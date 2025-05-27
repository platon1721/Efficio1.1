using App.DAL.Contracts;
using App.DAL.EF.Mappers;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories;

public class DepartmentRepository : BaseRepository<App.DAL.DTO.Department, App.Domain.Department>, IDepartmentRepository
{
    public DepartmentRepository(AppDbContext repositoryDbContext) : base(repositoryDbContext, new DepartmentUOWMapper())
    {
    }

    public async Task<IEnumerable<App.DAL.DTO.Department>> GetAllWithManagerAsync(bool noTracking = true)
    {
        var query = RepositoryDbSet.AsQueryable();

        if (noTracking)
        {
            query = query.AsNoTracking();
        }

        var domainDepartments = await query
            .Include(d => d.Manager)
            .ToListAsync();

        return domainDepartments.Select(d => Mapper.Map(d)).OfType<App.DAL.DTO.Department>();
    }

    public async Task<App.DAL.DTO.Department?> GetWithPersonsAsync(Guid id, bool noTracking = true)
    {
        var query = RepositoryDbSet.AsQueryable();

        if (noTracking)
        {
            query = query.AsNoTracking();
        }

        var domainDepartment = await query
            .Include(d => d.Manager)
            .Include(d => d.DepartmentPersons!)
                .ThenInclude(dp => dp.Person)
            .FirstOrDefaultAsync(d => d.Id.Equals(id));

        return Mapper.Map(domainDepartment);
    }

    public async Task<App.DAL.DTO.Department?> GetWithTasksAsync(Guid id, bool noTracking = true)
    {
        var query = RepositoryDbSet.AsQueryable();

        if (noTracking)
        {
            query = query.AsNoTracking();
        }

        var domainDepartment = await query
            .Include(d => d.Manager)
            .Include(d => d.Tasks!)
                .ThenInclude(t => t.TaskKeeper)
            .FirstOrDefaultAsync(d => d.Id.Equals(id));

        return Mapper.Map(domainDepartment);
    }

    public async Task<App.DAL.DTO.Department?> GetWithAllRelationsAsync(Guid id, bool noTracking = true)
    {
        var query = RepositoryDbSet.AsQueryable();

        if (noTracking)
        {
            query = query.AsNoTracking();
        }

        var domainDepartment = await query
            .Include(d => d.Manager)
            .Include(d => d.DepartmentPersons!)
                .ThenInclude(dp => dp.Person)
            .Include(d => d.Tasks!)
                .ThenInclude(t => t.TaskKeeper)
            .Include(d => d.PostDepartments!)
                .ThenInclude(pd => pd.Post)
            .Include(d => d.Feedbacks!) // Added Feedbacks to full relations
                .ThenInclude(f => f.FeedbackTags!)
                .ThenInclude(ft => ft.Tag)
            .FirstOrDefaultAsync(d => d.Id.Equals(id));

        return Mapper.Map(domainDepartment);
    }

    public async Task<IEnumerable<App.DAL.DTO.Department>> GetAllWithTasksAsync(bool noTracking = true)
    {
        var query = RepositoryDbSet.AsQueryable();

        if (noTracking)
        {
            query = query.AsNoTracking();
        }

        var domainDepartments = await query
            .Include(d => d.Manager)
            .Include(d => d.Tasks!)
                .ThenInclude(t => t.TaskKeeper)
            .ToListAsync();

        return domainDepartments.Select(d => Mapper.Map(d)).OfType<App.DAL.DTO.Department>();
    }

    // NEW METHODS FOR FEEDBACK SUPPORT
    public async Task<App.DAL.DTO.Department?> GetWithFeedbacksAsync(Guid id, bool noTracking = true)
    {
        var query = RepositoryDbSet.AsQueryable();

        if (noTracking)
        {
            query = query.AsNoTracking();
        }

        var domainDepartment = await query
            .Include(d => d.Manager)
            .Include(d => d.Feedbacks!)
                .ThenInclude(f => f.FeedbackTags!)
                .ThenInclude(ft => ft.Tag)
            .Include(d => d.Feedbacks!)
                .ThenInclude(f => f.Comments)
            .FirstOrDefaultAsync(d => d.Id.Equals(id));

        return Mapper.Map(domainDepartment);
    }

    public async Task<IEnumerable<App.DAL.DTO.Department>> GetAllWithFeedbacksAsync(bool noTracking = true)
    {
        var query = RepositoryDbSet.AsQueryable();

        if (noTracking)
        {
            query = query.AsNoTracking();
        }

        var domainDepartments = await query
            .Include(d => d.Manager)
            .Include(d => d.Feedbacks!)
                .ThenInclude(f => f.FeedbackTags!)
                .ThenInclude(ft => ft.Tag)
            .ToListAsync();

        return domainDepartments.Select(d => Mapper.Map(d)).OfType<App.DAL.DTO.Department>();
    }
}