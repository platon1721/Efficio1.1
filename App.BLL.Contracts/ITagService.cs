using App.DAL.Contracts;
using Base.BLL.Contracts;

namespace App.BLL.Contracts;

public interface ITagService : IBaseService<App.BLL.DTO.Tag>
{
    Task<IEnumerable<App.BLL.DTO.Tag>> GetTagsByTitleAsync(string title);
    Task<IEnumerable<App.BLL.DTO.Tag>> GetTagsByTitleContainsAsync(string searchTerm);
}