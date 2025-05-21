using App.DAL.Contracts;
using App.DAL.EF.Repositories;
using Base.DAL.EF;

namespace App.DAL.EF;

public class AppUOW : BaseUOW<AppDbContext>, IAppUOW
{
    public AppUOW(AppDbContext uowDbContext) : base(uowDbContext)
    {
    }

    private IPersonRepository? _personRepository;
    public IPersonRepository PersonRepository =>
        _personRepository ??= new PersonRepository(UOWDbContext);

    private IContactTypeRepository? _contactTypeRepository;
    public IContactTypeRepository ContactTypeRepository =>
        _contactTypeRepository ??= new ContactTypeRepository(UOWDbContext);

    private IContactRepository? _contactRepository;
    public IContactRepository ContactRepository =>
        _contactRepository ??= new ContactRepository(UOWDbContext);
}