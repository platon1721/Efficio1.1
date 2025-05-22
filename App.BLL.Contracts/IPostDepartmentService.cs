using Base.BLL.Contracts;
using App.DAL.Contracts;

namespace App.BLL.Contracts;

public interface IPostDepartmentService : IBaseService<App.BLL.DTO.PostDepartment>, IPostDepartmentRepositoryCustom
{
}