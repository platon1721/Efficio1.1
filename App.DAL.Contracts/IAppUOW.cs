using Base.DAL.Contracts;

namespace App.DAL.Contracts;

public interface IAppUOW : IBaseUOW
{
    IPersonRepository PersonRepository { get; }
    IContactRepository ContactRepository { get; }
    IContactTypeRepository ContactTypeRepository { get; }
    IDepartmentRepository DepartmentRepository { get; }
    IDepartmentPersonRepository DepartmentPersonRepository { get; }
    ITagRepository TagRepository { get; }
}