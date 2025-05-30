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

    public async Task<int> GetPersonCountByNameAsync(string name, Guid userId)
    {
        var count = await ServiceRepository.GetPersonCountByNameAsync(name, userId);
        return count;
    }
    
    public async Task<App.BLL.DTO.Person?> GetWithDepartmentsAsync(Guid id, Guid userId)
    {
        var result = await ServiceRepository.GetWithDepartmentsAsync(id, userId);
        return Mapper.Map(result);
    }
    
    public async Task<IEnumerable<App.BLL.DTO.Person>> GetAllByDepartmentAsync(Guid departmentId, Guid userId)
    {
        var result = await ServiceRepository.GetAllByDepartmentAsync(departmentId, userId);
        return result.Select(p => Mapper.Map(p)!);
    }
    
    public async Task<App.BLL.DTO.Person?> FindByUserIdAsync(Guid userId)
    {
        var persons = await ServiceRepository.AllAsync(userId);
        var person = persons.FirstOrDefault();
        return Mapper.Map(person);
    }

    public async Task<App.BLL.DTO.Person?> CreatePersonForUserAsync(Guid userId, string firstName, string lastName)
    {
        // Check if person already exists
        var existingPerson = await FindByUserIdAsync(userId);
        if (existingPerson != null)
        {
            return existingPerson;
        }

        // Create new person DTO
        var personDto = new App.BLL.DTO.Person // Fixed: was using DAL.DTO.Person
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            PersonName = $"{firstName} {lastName}".Trim()
        };

        var dalPersonDto = Mapper.Map(personDto);
        ServiceRepository.Add(dalPersonDto!, userId);
        await ServiceUOW.SaveChangesAsync();

        return personDto;
    }
}