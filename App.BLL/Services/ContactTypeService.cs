using App.BLL.Contracts;
using App.DAL.Contracts;
using App.DAL.DTO;
using Base.BLL;
using Base.BLL.Contracts;
using Base.Contracts;
using Base.DAL.Contracts;

namespace App.BLL.Services;

public class ContactTypeService : BaseService<App.BLL.DTO.ContactType, App.DAL.DTO.ContactType, App.DAL.Contracts.IContactTypeRepository>, IContactTypeService
{
    public ContactTypeService(
        IAppUOW serviceUOW, 
        IMapper<DTO.ContactType, ContactType> mapper) : base(serviceUOW, serviceUOW.ContactTypeRepository, mapper)
    {
    }
}