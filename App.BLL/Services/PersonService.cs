using App.BLL.Contracts;
using App.DAL.Contracts;
using App.DAL.DTO;
using Base.BLL;
using Base.BLL.Contracts;
using Base.Contracts;
using Base.DAL.Contracts;

namespace App.BLL.Services;

public class PersonService : BaseService<App.BLL.DTO.Person, App.DAL.DTO.Person, App.DAL.Contracts.IPersonRepository>, IPersonService
{
    public PersonService(
        IAppUOW serviceUOW,
        IMapper<DTO.Person, Person> mapper) : base(serviceUOW, serviceUOW.PersonRepository, mapper)
    {
    }

    public virtual async Task<int> GetPersonCountByNameAsync(string name, Guid userId)
    {
        var count = await ServiceRepository.GetPersonCountByNameAsync(name, userId);
        return count;
    }
}