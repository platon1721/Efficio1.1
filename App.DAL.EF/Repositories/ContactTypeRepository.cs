using App.DAL.Contracts;
using App.DAL.EF.Mappers;
using App.Domain;
using Base.DAL.EF;


namespace App.DAL.EF.Repositories;

public class ContactTypeRepository: BaseRepository<App.DAL.DTO.ContactType, App.Domain.ContactType>, IContactTypeRepository
{
    public ContactTypeRepository(AppDbContext repositoryDbContext) : base(repositoryDbContext, new ContactTypeUOWMapper())
    {
    }
}