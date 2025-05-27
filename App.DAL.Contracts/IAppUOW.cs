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
    IPostRepository PostRepository { get; }
    IPostTagRepository PostTagRepository { get; }
    IPostDepartmentRepository PostDepartmentRepository { get; }
    ITaskRepository TaskRepository { get; }
    IFeedbackRepository FeedbackRepository { get; }
    IFeedbackTagRepository FeedbackTagRepository { get; }
    ICommentRepository CommentRepository { get; }
}