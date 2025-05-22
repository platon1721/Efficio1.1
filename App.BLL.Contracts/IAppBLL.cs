using Base.BLL.Contracts;

namespace App.BLL.Contracts;

public interface IAppBLL : IBaseBLL
{
    IContactService ContactService { get; }
    IContactTypeService ContactTypeService { get; }
    IPersonService PersonService { get; }
    IDepartmentService DepartmentService { get; }
    IDepartmentPersonService DepartmentPersonService { get; }
    ITagService TagService { get; }
}
