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

    public async Task<IEnumerable<App.DAL.DTO.PostDepartment>> GetByPostIdAsync(Guid postId, bool noTracking = true)
    {
        return await ServiceRepository.GetByPostIdAsync(postId, noTracking);
    }

    public async Task<IEnumerable<App.DAL.DTO.PostDepartment>> GetByDepartmentIdAsync(Guid departmentId, bool noTracking = true)
    {
        return await ServiceRepository.GetByDepartmentIdAsync(departmentId, noTracking);
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