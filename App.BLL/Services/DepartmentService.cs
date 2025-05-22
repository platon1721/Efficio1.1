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
        var domainDepartments = await ServiceRepository.GetAllWithManagerAsync(noTracking);
        
        return domainDepartments.Select(d => new App.BLL.DTO.Department
        {
            Id = d.Id,
            DepartmentName = d.DepartmentName,
            ManagerId = d.ManagerId,
            Manager = d.Manager != null ? new App.BLL.DTO.Person
            {
                Id = d.Manager.Id,
                PersonName = d.Manager.PersonName
            } : null
        });
    }
    
    public async Task<App.BLL.DTO.Department?> GetWithPersonsAsync(Guid id, bool noTracking = true)
    {
        var domainDepartment = await ServiceRepository.GetWithPersonsAsync(id, noTracking);
        if (domainDepartment == null) return null;
        
        return new App.BLL.DTO.Department
        {
            Id = domainDepartment.Id,
            DepartmentName = domainDepartment.DepartmentName,
            ManagerId = domainDepartment.ManagerId,
            Manager = domainDepartment.Manager != null ? new App.BLL.DTO.Person
            {
                Id = domainDepartment.Manager.Id,
                PersonName = domainDepartment.Manager.PersonName
            } : null,
            Persons = domainDepartment.DepartmentPersons?.Select(dp => new App.BLL.DTO.Person
            {
                Id = dp.Person!.Id,
                PersonName = dp.Person.PersonName
            }).ToList()
        };
    }
}