using Base.DAL.Contracts;

namespace App.DAL.Contracts;

public interface IDepartmentPersonRepository : IBaseRepository<App.DAL.DTO.DepartmentPerson>, IDepartmentPersonRepositoryCustom
{
}

public interface IDepartmentPersonRepositoryCustom
{
    Task<IEnumerable<App.DAL.DTO.DepartmentPerson>> GetAllByDepartmentAsync(Guid departmentId, bool noTracking = true);
    Task<IEnumerable<App.DAL.DTO.DepartmentPerson>> GetAllByPersonAsync(Guid personId, bool noTracking = true);
}