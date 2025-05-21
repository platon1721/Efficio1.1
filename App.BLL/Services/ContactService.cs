using App.BLL.Contracts;
using App.DAL.Contracts;
using App.DAL.DTO;
using Base.BLL;
using Base.BLL.Contracts;
using Base.Contracts;
using Base.DAL.Contracts;

namespace App.BLL.Services;

public class ContactService : BaseService<App.BLL.DTO.Contact, App.DAL.DTO.Contact, App.DAL.Contracts.IContactRepository>, IContactService
{
    public ContactService(
        IAppUOW serviceUOW, 
        IMapper<DTO.Contact, Contact> mapper) : base(serviceUOW, serviceUOW.ContactRepository, mapper)
    {
    }
}