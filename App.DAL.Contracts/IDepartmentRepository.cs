using Base.DAL.Contracts;

namespace App.DAL.Contracts;

public interface IDepartmentRepository : IBaseRepository<App.DAL.DTO.Department>, IDepartmentRepositoryCustom
{
}

public interface IDepartmentRepositoryCustom
{
    Task<IEnumerable<App.Domain.Department>> GetAllWithManagerAsync(bool noTracking = true);
    Task<App.Domain.Department?> GetWithPersonsAsync(Guid id, bool noTracking = true);
}