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
    IPostService PostService { get; }
    IPostTagService PostTagService { get; }
    IPostDepartmentService PostDepartmentService { get; }
    ITaskService TaskService { get; }
    IFeedbackService FeedbackService { get; }
    ICommentService CommentService { get; }
    IFeedbackTagService FeedbackTagService { get; }
    IUserRegistrationService UserRegistrationService { get; }
}
