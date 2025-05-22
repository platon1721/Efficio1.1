using App.DAL.Contracts;
using App.DAL.EF.Mappers;
using Microsoft.EntityFrameworkCore;
using Base.DAL.EF;

namespace App.DAL.EF.Repositories;

public class DepartmentPersonRepository : BaseRepository<App.DAL.DTO.DepartmentPerson, App.Domain.DepartmentPerson>, IDepartmentPersonRepository
{
    public DepartmentPersonRepository(AppDbContext repositoryDbContext) : base(repositoryDbContext, new DepartmentPersonUOWMapper())
    {
    }
    
    public async Task<IEnumerable<App.DAL.DTO.DepartmentPerson>> GetAllByDepartmentAsync(Guid departmentId, bool noTracking = true)
    {
        var query = RepositoryDbSet.AsQueryable();
        
        if (noTracking)
        {
            query = query.AsNoTracking();
        }
        
        var domainEntities = await query
            .Include(dp => dp.Person)
            .Where(dp => dp.DepartmentId.Equals(departmentId))
            .ToListAsync();
            
        return domainEntities.Select(e => Mapper.Map(e)!);
    }
    
    public async Task<IEnumerable<App.DAL.DTO.DepartmentPerson>> GetAllByPersonAsync(Guid personId, bool noTracking = true)
    {
        var query = RepositoryDbSet.AsQueryable();
        
        if (noTracking)
        {
            query = query.AsNoTracking();
        }
        
        var domainEntities = await query
            .Include(dp => dp.Department)
            .Where(dp => dp.PersonId.Equals(personId))
            .ToListAsync();
            
        return domainEntities.Select(e => Mapper.Map(e)!);
    }
}