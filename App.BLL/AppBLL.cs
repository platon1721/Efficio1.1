using App.BLL.Contracts;
using App.BLL.Mappers;
using App.BLL.Services;
using App.DAL.Contracts;
using App.DAL.EF;
using Base.BLL;

namespace App.BLL;

public class AppBLL : BaseBLL<IAppUOW>, IAppBLL
{
    public AppBLL(IAppUOW uow) : base(uow)
    {
    }


    private IContactService? _contactService;
    public IContactService ContactService =>
        _contactService ??= new ContactService(
            BLLUOW, 
            new ContactBLLMapper()
        );

    
    private IContactTypeService? _contactTypeService;
    public IContactTypeService ContactTypeService =>
        _contactTypeService ??= new ContactTypeService(
            BLLUOW, 
            new ContactTypeBLLMapper()
        );
    
    private IPersonService? _personService;
    public IPersonService PersonService =>
        _personService ??= new PersonService(
            BLLUOW, 
            new PersonBLLMapper()
        );
        
   
}