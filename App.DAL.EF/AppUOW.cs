using App.DAL.Contracts;
using App.DAL.EF.Repositories;
using Base.DAL.EF;

namespace App.DAL.EF;

public class AppUOW : BaseUOW<AppDbContext>, IAppUOW
{
    public AppUOW(AppDbContext uowDbContext) : base(uowDbContext)
    {
    }

    private IPersonRepository? _personRepository;

    public IPersonRepository PersonRepository =>
        _personRepository ??= new PersonRepository(UOWDbContext);

    private IContactTypeRepository? _contactTypeRepository;

    public IContactTypeRepository ContactTypeRepository =>
        _contactTypeRepository ??= new ContactTypeRepository(UOWDbContext);

    private IContactRepository? _contactRepository;

    public IContactRepository ContactRepository =>
        _contactRepository ??= new ContactRepository(UOWDbContext);

    private IDepartmentRepository? _departmentRepository;

    public IDepartmentRepository DepartmentRepository =>
        _departmentRepository ??= new DepartmentRepository(UOWDbContext);

    private IDepartmentPersonRepository? _departmentPersonRepository;

    public IDepartmentPersonRepository DepartmentPersonRepository =>
        _departmentPersonRepository ??= new DepartmentPersonRepository(UOWDbContext);

    private ITagRepository? _tagRepository;

    public ITagRepository TagRepository =>
        _tagRepository ??= new TagRepository(UOWDbContext);
    
    private IPostRepository? _postRepository;
    public IPostRepository PostRepository =>
        _postRepository ??= new PostRepository(UOWDbContext);

    private IPostTagRepository? _postTagRepository;
    public IPostTagRepository PostTagRepository =>
        _postTagRepository ??= new PostTagRepository(UOWDbContext);

    private IPostDepartmentRepository? _postDepartmentRepository;
    public IPostDepartmentRepository PostDepartmentRepository =>
        _postDepartmentRepository ??= new PostDepartmentRepository(UOWDbContext);

    private ITaskRepository? _taskRepository; 
    public ITaskRepository TaskRepository =>
    _taskRepository ??= new TaskRepository(UOWDbContext);
    
    private IFeedbackRepository? _feedbackRepository;
    public IFeedbackRepository FeedbackRepository =>
        _feedbackRepository ??= new FeedbackRepository(UOWDbContext);

    private IFeedbackTagRepository? _feedbackTagRepository;
    public IFeedbackTagRepository FeedbackTagRepository =>
        _feedbackTagRepository ??= new FeedbackTagRepository(UOWDbContext);

    private ICommentRepository? _commentRepository;
    public ICommentRepository CommentRepository =>
        _commentRepository ??= new CommentRepository(UOWDbContext);
}