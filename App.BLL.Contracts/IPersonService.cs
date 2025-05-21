using App.DAL.Contracts;
using Base.BLL.Contracts;

namespace App.BLL.Contracts;

public interface IPersonService: IBaseService<App.BLL.DTO.Person>, IPersonRepositoryCustom
{
    
}