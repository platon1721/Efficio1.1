using App.BLL.Contracts;
using App.DAL.Contracts;
using Base.BLL;
using Base.Contracts;

namespace App.BLL.Services;

public class DepartmentPersonService : BaseService<App.BLL.DTO.DepartmentPerson, App.DAL.DTO.DepartmentPerson, App.DAL.Contracts.IDepartmentPersonRepository>, IDepartmentPersonService
{
    public DepartmentPersonService(
        IAppUOW serviceUOW,
        IMapper<App.BLL.DTO.DepartmentPerson, App.DAL.DTO.DepartmentPerson> mapper) 
        : base(serviceUOW, serviceUOW.DepartmentPersonRepository, mapper)
    {
    }
    
    public async Task<IEnumerable<App.DAL.DTO.DepartmentPerson>> GetAllByDepartmentAsync(Guid departmentId, bool noTracking = true)
    {
        var items = await ServiceRepository.GetAllByDepartmentAsync(departmentId, noTracking);
        return items;
    }
    
    public async Task<IEnumerable<App.DAL.DTO.DepartmentPerson>> GetAllByPersonAsync(Guid personId, bool noTracking = true)
    {
        var items = await ServiceRepository.GetAllByPersonAsync(personId, noTracking);
        return items;
    }
}