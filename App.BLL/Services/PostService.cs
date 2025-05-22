using App.BLL.Contracts;
using App.BLL.Mappers;
using App.DAL.Contracts;
using Base.BLL;
using Base.Contracts;

namespace App.BLL.Services;

public class PostService : BaseService<App.BLL.DTO.Post, App.DAL.DTO.Post, App.DAL.Contracts.IPostRepository>, IPostService
{
    private readonly IPostTagService _postTagService;
    private readonly IPostDepartmentService _postDepartmentService;

    public PostService(
        IAppUOW serviceUOW,
        IMapper<App.BLL.DTO.Post, App.DAL.DTO.Post> mapper,
        IPostTagService postTagService,
        IPostDepartmentService postDepartmentService) 
        : base(serviceUOW, serviceUOW.PostRepository, mapper)
    {
        _postTagService = postTagService;
        _postDepartmentService = postDepartmentService;
    }

    public async Task<IEnumerable<App.DAL.DTO.Post>> GetAllWithTagsAndDepartmentsAsync(bool noTracking = true)
    {
        return await ServiceRepository.GetAllWithTagsAndDepartmentsAsync(noTracking);
    }

    public async Task<App.DAL.DTO.Post?> GetWithTagsAndDepartmentsAsync(Guid id, bool noTracking = true)
    {
        return await ServiceRepository.GetWithTagsAndDepartmentsAsync(id, noTracking);
    }

    public async Task<IEnumerable<App.DAL.DTO.Post>> GetByTagAsync(Guid tagId, bool noTracking = true)
    {
        return await ServiceRepository.GetByTagAsync(tagId, noTracking);
    }

    public async Task<IEnumerable<App.DAL.DTO.Post>> GetByDepartmentAsync(Guid departmentId, bool noTracking = true)
    {
        return await ServiceRepository.GetByDepartmentAsync(departmentId, noTracking);
    }

    public async Task<App.BLL.DTO.Post?> CreatePostWithTagsAndDepartmentsAsync(
        App.BLL.DTO.Post post, 
        IEnumerable<Guid> tagIds, 
        IEnumerable<Guid> departmentIds)
    {
        // First create the post
        Add(post);
        await ServiceUOW.SaveChangesAsync();

        // Add tags
        foreach (var tagId in tagIds)
        {
            await AddTagToPostAsync(post.Id, tagId);
        }

        // Add departments  
        foreach (var departmentId in departmentIds)
        {
            await AddDepartmentToPostAsync(post.Id, departmentId);
        }

        await ServiceUOW.SaveChangesAsync();
        
        // Return the post with all relationships
        var dalPost = await GetWithTagsAndDepartmentsAsync(post.Id);
        return dalPost != null ? Mapper.Map(dalPost) : null;
    }

    public async Task<bool> UpdatePostTagsAsync(Guid postId, IEnumerable<Guid> tagIds)
    {
        // Get existing tags for this post
        var existingPostTags = await _postTagService.GetByPostIdAsync(postId);
        var existingTagIds = existingPostTags.Select(pt => pt.TagId).ToList();
        
        var newTagIds = tagIds.ToList();

        // Remove tags that are no longer needed
        var tagsToRemove = existingTagIds.Except(newTagIds);
        foreach (var tagId in tagsToRemove)
        {
            await RemoveTagFromPostAsync(postId, tagId);
        }

        // Add new tags
        var tagsToAdd = newTagIds.Except(existingTagIds);
        foreach (var tagId in tagsToAdd)
        {
            await AddTagToPostAsync(postId, tagId);
        }

        await ServiceUOW.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdatePostDepartmentsAsync(Guid postId, IEnumerable<Guid> departmentIds)
    {
        // Get existing departments for this post
        var existingPostDepartments = await _postDepartmentService.GetByPostIdAsync(postId);
        var existingDepartmentIds = existingPostDepartments.Select(pd => pd.DepartmentId).ToList();
        
        var newDepartmentIds = departmentIds.ToList();

        // Remove departments that are no longer needed
        var departmentsToRemove = existingDepartmentIds.Except(newDepartmentIds);
        foreach (var departmentId in departmentsToRemove)
        {
            await RemoveDepartmentFromPostAsync(postId, departmentId);
        }

        // Add new departments
        var departmentsToAdd = newDepartmentIds.Except(existingDepartmentIds);
        foreach (var departmentId in departmentsToAdd)
        {
            await AddDepartmentToPostAsync(postId, departmentId);
        }

        await ServiceUOW.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AddTagToPostAsync(Guid postId, Guid tagId)
    {
        // Check if relationship already exists
        var existingPostTags = await _postTagService.GetByPostIdAsync(postId);
        if (existingPostTags.Any(pt => pt.TagId == tagId))
        {
            return false; // Already exists
        }

        var postTag = new App.BLL.DTO.PostTag
        {
            Id = Guid.NewGuid(),
            PostId = postId,
            TagId = tagId
        };

        _postTagService.Add(postTag);
        return true;
    }

    public async Task<bool> RemoveTagFromPostAsync(Guid postId, Guid tagId)
    {
        var existingPostTags = await _postTagService.GetByPostIdAsync(postId);
        var postTagToRemove = existingPostTags.FirstOrDefault(pt => pt.TagId == tagId);
        
        if (postTagToRemove != null)
        {
            _postTagService.Remove(postTagToRemove.Id);
            return true;
        }
        
        return false;
    }

    public async Task<bool> AddDepartmentToPostAsync(Guid postId, Guid departmentId)
    {
        // Check if relationship already exists
        var existingPostDepartments = await _postDepartmentService.GetByPostIdAsync(postId);
        if (existingPostDepartments.Any(pd => pd.DepartmentId == departmentId))
        {
            return false; // Already exists
        }

        var postDepartment = new App.BLL.DTO.PostDepartment
        {
            Id = Guid.NewGuid(),
            PostId = postId,
            DepartmentId = departmentId
        };

        _postDepartmentService.Add(postDepartment);
        return true;
    }

    public async Task<bool> RemoveDepartmentFromPostAsync(Guid postId, Guid departmentId)
    {
        var existingPostDepartments = await _postDepartmentService.GetByPostIdAsync(postId);
        var postDepartmentToRemove = existingPostDepartments.FirstOrDefault(pd => pd.DepartmentId == departmentId);
        
        if (postDepartmentToRemove != null)
        {
            _postDepartmentService.Remove(postDepartmentToRemove.Id);
            return true;
        }
        
        return false;
    }
}