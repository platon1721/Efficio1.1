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
    
    public virtual async Task<App.DAL.DTO.Person?> GetWithDepartmentsAsync(Guid id, Guid userId)
    {
        var result = await ServiceRepository.GetWithDepartmentsAsync(id, userId);
        return result;
    }
    
    public virtual async Task<IEnumerable<App.DAL.DTO.Person>> GetAllByDepartmentAsync(Guid departmentId, Guid userId)
    {
        var result = await ServiceRepository.GetAllByDepartmentAsync(departmentId, userId);
        return result;
    }
}