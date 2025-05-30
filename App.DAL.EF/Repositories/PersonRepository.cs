using App.DAL.Contracts;
using App.DAL.DTO;
using App.DAL.EF.Mappers;
using Base.DAL.EF;

using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories;

public class PersonRepository :BaseRepository<App.DAL.DTO.Person, App.Domain.Person>, IPersonRepository
{
    public PersonRepository(AppDbContext repositoryDbContext) : base(repositoryDbContext, new PersonUOWMapper())
    {
    }

    public override async  Task<IEnumerable<App.DAL.DTO.Person>> AllAsync(Guid userId = default)
    {
        var query = GetQuery(userId);
        query = query.Include(p => p.User);
        return (await query.ToListAsync()).Select(e => Mapper.Map(e)!);
    }

    public async Task<int> GetPersonCountByNameAsync(string name, Guid userId)
    {
        var query = GetQuery(userId);
        return await query
            .CountAsync(p => p.PersonName.ToUpper().Contains(name.Trim().ToUpper()));
    }
    
    public async Task<App.DAL.DTO.Person?> GetWithDepartmentsAsync(Guid id, Guid userId)
    {
        var query = GetQuery(userId);
        var domainEntity = await query
            .Include(p => p.DepartmentPersons!)
            .ThenInclude(dp => dp.Department)
            .FirstOrDefaultAsync(p => p.Id.Equals(id));
            
        return Mapper.Map(domainEntity);
    }
    
    public async Task<IEnumerable<App.DAL.DTO.Person>> GetAllByDepartmentAsync(Guid departmentId, Guid userId)
    {
        var query = GetQuery(userId);
        var domainEntities = await query
            .Include(p => p.DepartmentPersons!)
            .Where(p => p.DepartmentPersons!.Any(dp => dp.DepartmentId.Equals(departmentId)))
            .ToListAsync();
            
        return domainEntities.Select(e => Mapper.Map(e)!);
    }
    
    public async Task<Person?> FindByUserIdAsync(Guid userId)
    {
        var res = await RepositoryDbSet
            .FirstOrDefaultAsync(p => p.UserId == userId);
        return Mapper.Map(res);
    }
}