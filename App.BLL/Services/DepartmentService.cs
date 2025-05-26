using App.BLL.Contracts;
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
        var dalItems = await ServiceRepository.GetAllWithManagerAsync(noTracking);
        return dalItems.Select(item => Mapper.Map(item)!);
    }

    public async Task<App.BLL.DTO.Department?> GetWithPersonsAsync(Guid id, bool noTracking = true)
    {
        var dalItem = await ServiceRepository.GetWithPersonsAsync(id, noTracking);
        return Mapper.Map(dalItem);
    }

    public async Task<App.BLL.DTO.Department?> GetWithTasksAsync(Guid id, bool noTracking = true)
    {
        var dalItem = await ServiceRepository.GetWithTasksAsync(id, noTracking);
        return Mapper.Map(dalItem);
    }

    public async Task<App.BLL.DTO.Department?> GetWithAllRelationsAsync(Guid id, bool noTracking = true)
    {
        var dalItem = await ServiceRepository.GetWithAllRelationsAsync(id, noTracking);
        return Mapper.Map(dalItem);
    }

    public async Task<IEnumerable<App.BLL.DTO.Department>> GetAllWithTasksAsync(bool noTracking = true)
    {
        var dalItems = await ServiceRepository.GetAllWithTasksAsync(noTracking);
        return dalItems.Select(item => Mapper.Map(item)!);
    }
}