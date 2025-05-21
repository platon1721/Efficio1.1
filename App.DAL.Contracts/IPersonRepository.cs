using App.Domain;
using Base.DAL.Contracts;


namespace App.DAL.Contracts;

public interface IPersonRepository: IBaseRepository<App.DAL.DTO.Person>, IPersonRepositoryCustom
{
}

public interface IPersonRepositoryCustom
{
    Task<int> GetPersonCountByNameAsync(string name, Guid userId);
}