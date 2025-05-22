using App.BLL.Contracts;
using App.BLL.Mappers;
using App.DAL.Contracts;
using Base.BLL;
using Base.Contracts;

namespace App.BLL.Services;

public class PostDepartmentService : BaseService<App.BLL.DTO.PostDepartment, App.DAL.DTO.PostDepartment, App.DAL.Contracts.IPostDepartmentRepository>, IPostDepartmentService
{
    public PostDepartmentService(
        IAppUOW serviceUOW,
        IMapper<App.BLL.DTO.PostDepartment, App.DAL.DTO.PostDepartment> mapper) 
        : base(serviceUOW, serviceUOW.PostDepartmentRepository, mapper)
    {
    }

    public async Task<IEnumerable<App.BLL.DTO.PostDepartment>> GetByPostIdAsync(Guid postId, bool noTracking = true)
    {
        var dalPostDepartments = await ServiceRepository.GetByPostIdAsync(postId, noTracking);
        return dalPostDepartments.Select(pd => Mapper.Map(pd)!);
    }

    public async Task<IEnumerable<App.BLL.DTO.PostDepartment>> GetByDepartmentIdAsync(Guid departmentId, bool noTracking = true)
    {
        var dalPostDepartments = await ServiceRepository.GetByDepartmentIdAsync(departmentId, noTracking);
        return dalPostDepartments.Select(pd => Mapper.Map(pd)!);
    }

    public async Task<bool> PostDepartmentExistsAsync(Guid postId, Guid departmentId)
    {
        return await ServiceRepository.PostDepartmentExistsAsync(postId, departmentId);
    }

    public async Task RemoveByPostAndDepartmentAsync(Guid postId, Guid departmentId)
    {
        await ServiceRepository.RemoveByPostAndDepartmentAsync(postId, departmentId);
    }
}