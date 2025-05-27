using App.BLL.Contracts;
using App.BLL.Mappers;
using App.DAL.Contracts;
using Base.BLL;
using Base.Contracts;

namespace App.BLL.Services;

public class CommentService : BaseService<App.BLL.DTO.Comment, App.DAL.DTO.Comment, App.DAL.Contracts.ICommentRepository>, ICommentService
{
    public CommentService(
        IAppUOW serviceUOW,
        IMapper<App.BLL.DTO.Comment, App.DAL.DTO.Comment> mapper) 
        : base(serviceUOW, serviceUOW.CommentRepository, mapper)
    {
    }

    public async Task<IEnumerable<App.BLL.DTO.Comment>> GetByPostIdAsync(Guid postId, bool noTracking = true)
    {
        var dalComments = await ServiceRepository.GetByPostIdAsync(postId, noTracking);
        return dalComments.Select(c => Mapper.Map(c)!);
    }

    public async Task<IEnumerable<App.BLL.DTO.Comment>> GetByFeedbackIdAsync(Guid feedbackId, bool noTracking = true)
    {
        var dalComments = await ServiceRepository.GetByFeedbackIdAsync(feedbackId, noTracking);
        return dalComments.Select(c => Mapper.Map(c)!);
    }

    public async Task<App.BLL.DTO.Comment?> AddCommentToPostAsync(Guid postId, string content)
    {
        // Business validation
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("Comment content cannot be empty");
        }

        if (content.Length > 1000)
        {
            throw new ArgumentException("Comment content cannot exceed 1000 characters");
        }

        var comment = new App.BLL.DTO.Comment
        {
            Id = Guid.NewGuid(),
            PostId = postId,
            FeedbackId = null,
            Content = content.Trim()
        };

        // Validate comment business rules
        if (!await ValidateCommentAsync(comment))
        {
            return null;
        }

        Add(comment);
        await ServiceUOW.SaveChangesAsync();

        return await FindAsync(comment.Id);
    }

    public async Task<App.BLL.DTO.Comment?> AddCommentToFeedbackAsync(Guid feedbackId, string content)
    {
        // Business validation
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("Comment content cannot be empty");
        }

        if (content.Length > 1000)
        {
            throw new ArgumentException("Comment content cannot exceed 1000 characters");
        }

        var comment = new App.BLL.DTO.Comment
        {
            Id = Guid.NewGuid(),
            PostId = null,
            FeedbackId = feedbackId,
            Content = content.Trim()
        };

        // Validate comment business rules
        if (!await ValidateCommentAsync(comment))
        {
            return null;
        }

        Add(comment);
        await ServiceUOW.SaveChangesAsync();

        return await FindAsync(comment.Id);
    }

    public async Task<bool> ValidateCommentAsync(App.BLL.DTO.Comment comment)
    {
        // Business rule: Comment must belong to either a Post OR Feedback, but not both
        if (comment.PostId.HasValue && comment.FeedbackId.HasValue)
        {
            return false; // Cannot belong to both
        }

        if (!comment.PostId.HasValue && !comment.FeedbackId.HasValue)
        {
            return false; // Must belong to at least one
        }

        // Additional business validations can be added here
        // For example: check if post/feedback exists, user permissions, etc.

        return await Task.FromResult(true);
    }
}