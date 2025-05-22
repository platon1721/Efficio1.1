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
    
    public async Task<IEnumerable<App.Domain.Department>> GetAllWithManagerAsync(bool noTracking = true)
    {
        var query = RepositoryDbSet.AsQueryable();
        
        if (noTracking)
        {
            query = query.AsNoTracking();
        }
        
        return await query
            .Include(d => d.Manager)
            .ToListAsync();
    }
    
    public async Task<App.Domain.Department?> GetWithPersonsAsync(Guid id, bool noTracking = true)
    {
        var query = RepositoryDbSet.AsQueryable();
        
        if (noTracking)
        {
            query = query.AsNoTracking();
        }
        
        return await query
            .Include(d => d.Manager)
            .Include(d => d.DepartmentPersons!)
            .ThenInclude(dp => dp.Person)
            .FirstOrDefaultAsync(d => d.Id.Equals(id));
    }
}