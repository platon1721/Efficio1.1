using App.DAL.Contracts;
using Base.BLL.Contracts;

namespace App.BLL.Contracts;

public interface IPersonService: IBaseService<App.BLL.DTO.Person>
{
    Task<App.BLL.DTO.Person?> FindByUserIdAsync(Guid userId);
    Task<App.BLL.DTO.Person?> CreatePersonForUserAsync(Guid userId, string firstName, string lastName);

    
    Task<App.BLL.DTO.Person?> GetWithDepartmentsAsync(Guid id, Guid userId);
    Task<IEnumerable<App.BLL.DTO.Person>> GetAllByDepartmentAsync(Guid departmentId, Guid userId);
}