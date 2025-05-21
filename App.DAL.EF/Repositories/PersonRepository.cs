using App.DAL.Contracts;
using App.DAL.EF.Mappers;
using App.Domain;
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
}