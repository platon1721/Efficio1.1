using App.DAL.Contracts;
using App.DAL.EF.Mappers;
using App.Domain;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories;

public class ContactRepository : BaseRepository<App.DAL.DTO.Contact, App.Domain.Contact>, IContactRepository
{
    public ContactRepository(AppDbContext repositoryDbContext) : base(repositoryDbContext, new ContactUOWMapper())
    {
    }

    public override async Task<IEnumerable<App.DAL.DTO.Contact>> AllAsync(Guid userId = default)
    {
        return (await RepositoryDbSet
            .Include(c => c.Person)
            .Include(c => c.ContactType)
            .Where(c => c.Person!.UserId == userId)
            .ToListAsync()).Select(e => Mapper.Map(e)!);
    }

    public override async Task<App.DAL.DTO.Contact?> FindAsync(Guid id, Guid userId = default)
    {
        return Mapper.Map(await RepositoryDbSet
            .Include(c => c.Person)
            .Include(c => c.ContactType)
            .Where(c => c.Id == id && c.Person!.UserId == userId)
            .FirstOrDefaultAsync());
    }
    
    // TODO - override add and addAsync to check for data ownership
    
}