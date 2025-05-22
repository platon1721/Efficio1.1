using Base.DAL.Contracts;

namespace App.DAL.Contracts;

public interface ITagRepository: IBaseRepository<App.DAL.DTO.Tag>, ITagRepositoryCustom
{
}

public interface ITagRepositoryCustom
{
    Task<IEnumerable<App.DAL.DTO.Tag>> GetTagsByTitleAsync(string title);
    Task<IEnumerable<App.DAL.DTO.Tag>> GetTagsByTitleContainsAsync(string searchTerm);
}