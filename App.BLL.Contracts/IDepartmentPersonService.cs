using App.BLL.DTO;
using App.DAL.Contracts;
using Base.BLL.Contracts;

namespace App.BLL.Contracts;

public interface IDepartmentPersonService : IBaseService<App.BLL.DTO.DepartmentPerson>
{
    Task<IEnumerable<App.BLL.DTO.DepartmentPerson>> GetAllByDepartmentAsync(Guid departmentId, bool noTracking = true);
    Task<IEnumerable<App.BLL.DTO.DepartmentPerson>> GetAllByPersonAsync(Guid personId, bool noTracking = true);
}