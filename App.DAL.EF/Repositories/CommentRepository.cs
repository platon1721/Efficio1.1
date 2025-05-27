using App.DAL.Contracts;
using App.DAL.EF.Mappers;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories;

public class CommentRepository : BaseRepository<App.DAL.DTO.Comment, App.Domain.Comment>, ICommentRepository
{
    public CommentRepository(AppDbContext repositoryDbContext) : base(repositoryDbContext, new CommentUOWMapper())
    {
    }

    public async Task<IEnumerable<App.DAL.DTO.Comment>> GetByPostIdAsync(Guid postId, bool noTracking = true)
    {
        var query = RepositoryDbSet.AsQueryable();
        
        if (noTracking)
        {
            query = query.AsNoTracking();
        }
        
        var domainComments = await query
            .Where(c => c.PostId == postId)
            .Include(c => c.Post)
            .OrderBy(c => c.CreatedAt) // Comments usually ordered by creation time
            .ToListAsync();
            
        return domainComments.Select(c => Mapper.Map(c)).OfType<App.DAL.DTO.Comment>();
    }
    
    public async Task<IEnumerable<App.DAL.DTO.Comment>> GetByFeedbackIdAsync(Guid feedbackId, bool noTracking = true)
    {
        var query = RepositoryDbSet.AsQueryable();
        
        if (noTracking)
        {
            query = query.AsNoTracking();
        }
        
        var domainComments = await query
            .Where(c => c.FeedbackId == feedbackId)
            .Include(c => c.Feedback)
            .OrderBy(c => c.CreatedAt) // Comments usually ordered by creation time
            .ToListAsync();
            
        return domainComments.Select(c => Mapper.Map(c)).OfType<App.DAL.DTO.Comment>();
    }

    // Override FindAsync to include related entities
    public override async Task<App.DAL.DTO.Comment?> FindAsync(Guid id, Guid userId = default)
    {
        var domainComment = await RepositoryDbSet
            .Include(c => c.Post)
            .Include(c => c.Feedback)
            .FirstOrDefaultAsync(c => c.Id == id);
            
        return Mapper.Map(domainComment);
    }
}