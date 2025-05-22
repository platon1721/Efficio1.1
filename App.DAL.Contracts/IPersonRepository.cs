using App.Domain;
using Base.DAL.Contracts;


namespace App.DAL.Contracts;

public interface IPersonRepository: IBaseRepository<App.DAL.DTO.Person>, IPersonRepositoryCustom
{
}

public interface IPersonRepositoryCustom
{
    Task<int> GetPersonCountByNameAsync(string name, Guid userId);
    Task<App.DAL.DTO.Person?> GetWithDepartmentsAsync(Guid id, Guid userId);
    Task<IEnumerable<App.DAL.DTO.Person>> GetAllByDepartmentAsync(Guid departmentId, Guid userId);
}