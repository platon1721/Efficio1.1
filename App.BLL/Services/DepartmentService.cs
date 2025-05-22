using App.BLL.Contracts;
using App.BLL.Mappers;
using App.DAL.Contracts;
using Base.BLL;
using Base.Contracts;

namespace App.BLL.Services;

public class DepartmentService : BaseService<App.BLL.DTO.Department, App.DAL.DTO.Department, App.DAL.Contracts.IDepartmentRepository>, IDepartmentService
{
    public DepartmentService(
        IAppUOW serviceUOW,
        IMapper<App.BLL.DTO.Department, App.DAL.DTO.Department> mapper) 
        : base(serviceUOW, serviceUOW.DepartmentRepository, mapper)
    {
    }
    
    public async Task<IEnumerable<App.BLL.DTO.Department>> GetAllWithManagerAsync(bool noTracking = true)
    {
        var departments = await ServiceRepository.GetAllWithManagerAsync(noTracking);
        
        return departments
            .Select(d => new App.DAL.DTO.Department
            {
                Id = d.Id,
                DepartmentName = d.DepartmentName,
                ManagerId = d.ManagerId,
                Manager = d.Manager != null ? new App.DAL.DTO.Person
                {
                    Id = d.Manager.Id,
                    PersonName = d.Manager.PersonName
                } : null
            })
            .Select(d => Mapper.Map(d)!);
    }
    
    public async Task<App.BLL.DTO.Department?> GetWithPersonsAsync(Guid id, bool noTracking = true)
    {
        var department = await ServiceRepository.GetWithPersonsAsync(id, noTracking);
        if (department == null) return null;
        
        var dalDto = new App.DAL.DTO.Department
        {
            Id = department.Id,
            DepartmentName = department.DepartmentName,
            ManagerId = department.ManagerId,
            Manager = department.Manager != null ? new App.DAL.DTO.Person
            {
                Id = department.Manager.Id,
                PersonName = department.Manager.PersonName
            } : null,
            Persons = department.DepartmentPersons?.Select(dp => new App.DAL.DTO.Person
            {
                Id = dp.Person!.Id,
                PersonName = dp.Person.PersonName
            }).ToList()
        };
        
        return Mapper.Map(dalDto);
    }
}
