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
    
    private IPostService? _postService;
    public IPostService PostService =>
        _postService ??= new PostService(
            BLLUOW,
            new PostBLLMapper(),
            PostTagService,
            PostDepartmentService);
    
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
}