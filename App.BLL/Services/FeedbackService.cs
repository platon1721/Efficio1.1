using App.BLL.Contracts;
using App.BLL.Mappers;
using App.DAL.Contracts;
using Base.BLL;
using Base.Contracts;

namespace App.BLL.Services;

public class FeedbackService : BaseService<App.BLL.DTO.Feedback, App.DAL.DTO.Feedback, App.DAL.Contracts.IFeedbackRepository>, IFeedbackService
{
    private readonly IFeedbackTagService _feedbackTagService;

    public FeedbackService(
        IAppUOW serviceUOW,
        IMapper<App.BLL.DTO.Feedback, App.DAL.DTO.Feedback> mapper,
        IFeedbackTagService feedbackTagService) 
        : base(serviceUOW, serviceUOW.FeedbackRepository, mapper)
    {
        _feedbackTagService = feedbackTagService;
    }

    public async Task<IEnumerable<App.BLL.DTO.Feedback>> GetAllWithTagsAndCommentsAsync(bool noTracking = true)
    {
        var dalFeedbacks = await ServiceRepository.GetAllWithTagsAndCommentsAsync(noTracking);
        return dalFeedbacks.Select(f => Mapper.Map(f)!);
    }

    public async Task<App.BLL.DTO.Feedback?> GetWithTagsAndCommentsAsync(Guid id, bool noTracking = true)
    {
        var dalFeedback = await ServiceRepository.GetWithTagsAndCommentsAsync(id, noTracking);
        return Mapper.Map(dalFeedback);
    }

    public async Task<IEnumerable<App.BLL.DTO.Feedback>> GetByDepartmentAsync(Guid departmentId, bool noTracking = true)
    {
        var dalFeedbacks = await ServiceRepository.GetByDepartmentAsync(departmentId, noTracking);
        return dalFeedbacks.Select(f => Mapper.Map(f)!);
    }

    public async Task<IEnumerable<App.BLL.DTO.Feedback>> GetByTagAsync(Guid tagId, bool noTracking = true)
    {
        var dalFeedbacks = await ServiceRepository.GetByTagAsync(tagId, noTracking);
        return dalFeedbacks.Select(f => Mapper.Map(f)!);
    }

    public async Task<App.BLL.DTO.Feedback?> CreateFeedbackWithTagsAsync(
        App.BLL.DTO.Feedback feedback, 
        IEnumerable<Guid> tagIds)
    {
        // First create the feedback
        Add(feedback);
        await ServiceUOW.SaveChangesAsync();

        // Add tags
        foreach (var tagId in tagIds)
        {
            await AddTagToFeedbackAsync(feedback.Id, tagId);
        }

        await ServiceUOW.SaveChangesAsync();
        
        // Return the feedback with all relationships
        return await GetWithTagsAndCommentsAsync(feedback.Id);
    }

    public async Task<bool> UpdateFeedbackTagsAsync(Guid feedbackId, IEnumerable<Guid> tagIds)
    {
        // Get existing tags for this feedback
        var existingFeedbackTags = await _feedbackTagService.GetByFeedbackIdAsync(feedbackId);
        var existingTagIds = existingFeedbackTags.Select(ft => ft.TagId).ToList();
        
        var newTagIds = tagIds.ToList();

        // Remove tags that are no longer needed
        var tagsToRemove = existingTagIds.Except(newTagIds);
        foreach (var tagId in tagsToRemove)
        {
            await RemoveTagFromFeedbackAsync(feedbackId, tagId);
        }

        // Add new tags
        var tagsToAdd = newTagIds.Except(existingTagIds);
        foreach (var tagId in tagsToAdd)
        {
            await AddTagToFeedbackAsync(feedbackId, tagId);
        }

        await ServiceUOW.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AddTagToFeedbackAsync(Guid feedbackId, Guid tagId)
    {
        // Check if relationship already exists
        var existingFeedbackTags = await _feedbackTagService.GetByFeedbackIdAsync(feedbackId);
        if (existingFeedbackTags.Any(ft => ft.TagId == tagId))
        {
            return false; // Already exists
        }

        var feedbackTag = new App.BLL.DTO.FeedbackTag
        {
            Id = Guid.NewGuid(),
            FeedbackId = feedbackId,
            TagId = tagId
        };

        _feedbackTagService.Add(feedbackTag);
        return true;
    }

    public async Task<bool> RemoveTagFromFeedbackAsync(Guid feedbackId, Guid tagId)
    {
        var existingFeedbackTags = await _feedbackTagService.GetByFeedbackIdAsync(feedbackId);
        var feedbackTagToRemove = existingFeedbackTags.FirstOrDefault(ft => ft.TagId == tagId);
        
        if (feedbackTagToRemove != null)
        {
            _feedbackTagService.Remove(feedbackTagToRemove.Id);
            return true;
        }
        
        return false;
    }
}