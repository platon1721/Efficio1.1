// App.BLL/AppBLL.cs - Updated with Comment Service dependency in PostService
using App.BLL.Contracts;
using App.BLL.Mappers;
using App.BLL.Services;
using App.DAL.Contracts;
using App.DAL.EF;
using Base.BLL;

namespace App.BLL;

public class AppBLL : BaseBLL<IAppUOW>, IAppBLL
{
    public AppBLL(IAppUOW uow) : base(uow)
    {
    }

    private IContactService? _contactService;
    public IContactService ContactService =>
        _contactService ??= new ContactService(
            BLLUOW, 
            new ContactBLLMapper()
        );

    private IContactTypeService? _contactTypeService;
    public IContactTypeService ContactTypeService =>
        _contactTypeService ??= new ContactTypeService(
            BLLUOW, 
            new ContactTypeBLLMapper()
        );
    
    private IPersonService? _personService;
    public IPersonService PersonService =>
        _personService ??= new PersonService(
            BLLUOW, 
            new PersonBLLMapper()
        );
        
    private IDepartmentService? _departmentService;
    public IDepartmentService DepartmentService =>
        _departmentService ??= new DepartmentService(
            BLLUOW,
            new DepartmentBLLMapper());
        
    private IDepartmentPersonService? _departmentPersonService;
    public IDepartmentPersonService DepartmentPersonService =>
        _departmentPersonService ??= new DepartmentPersonService(
            BLLUOW,
            new DepartmentPersonBLLMapper());
    
    private ITagService? _tagService;
    public ITagService TagService =>
        _tagService ??= new TagService(
            BLLUOW,
            new TagBLLMapper());
    
    private ICommentService? _commentService;
    public ICommentService CommentService =>
        _commentService ??= new CommentService(
            BLLUOW,
            new CommentBLLMapper());
    
    private IPostService? _postService;
    public IPostService PostService =>
        _postService ??= new PostService(
            BLLUOW,
            new PostBLLMapper(),
            PostTagService,
            PostDepartmentService,
            CommentService); 
    
    private IPostTagService? _postTagService;
    public IPostTagService PostTagService =>
        _postTagService ??= new PostTagService(
            BLLUOW,
            new PostTagBLLMapper());
    
    private IPostDepartmentService? _postDepartmentService;
    public IPostDepartmentService PostDepartmentService =>
        _postDepartmentService ??= new PostDepartmentService(
            BLLUOW,
            new PostDepartmentBLLMapper());
    
    private ITaskService? _taskService;
    public ITaskService TaskService =>
        _taskService ??= new TaskService(
            BLLUOW,
            new TaskBLLMapper());

    private IFeedbackTagService? _feedbackTagService;
    public IFeedbackTagService FeedbackTagService =>
        _feedbackTagService ??= new FeedbackTagService(
            BLLUOW,
            new FeedbackTagBLLMapper());
    
    private IFeedbackService? _feedbackService;
    public IFeedbackService FeedbackService =>
        _feedbackService ??= new FeedbackService(
            BLLUOW,
            new FeedbackBLLMapper(),
            FeedbackTagService); 
}