using Base.BLL.Contracts;
using App.DAL.Contracts;

namespace App.BLL.Contracts;

public interface IPostTagService : IBaseService<App.BLL.DTO.PostTag>, IPostTagRepositoryCustom
{
}