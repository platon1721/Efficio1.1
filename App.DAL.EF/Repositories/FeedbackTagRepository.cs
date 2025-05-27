using App.DAL.Contracts;
using App.DAL.EF.Mappers;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories;

public class FeedbackTagRepository : BaseRepository<App.DAL.DTO.FeedbackTag, App.Domain.FeedbackTag>, IFeedbackTagRepository
{
    public FeedbackTagRepository(AppDbContext repositoryDbContext) : base(repositoryDbContext, new FeedbackTagUOWMapper())
    {
    }

    public async Task<IEnumerable<App.DAL.DTO.FeedbackTag>> GetByFeedbackIdAsync(Guid feedbackId, bool noTracking = true)
    {
        var query = RepositoryDbSet.AsQueryable();
        
        if (noTracking)
        {
            query = query.AsNoTracking();
        }
        
        var domainFeedbackTags = await query
            .Where(ft => ft.FeedbackId == feedbackId)
            .Include(ft => ft.Feedback)
            .Include(ft => ft.Tag)
            .ToListAsync();
            
        return domainFeedbackTags.Select(ft => Mapper.Map(ft)).OfType<App.DAL.DTO.FeedbackTag>();
    }
    
    public async Task<IEnumerable<App.DAL.DTO.FeedbackTag>> GetByTagIdAsync(Guid tagId, bool noTracking = true)
    {
        var query = RepositoryDbSet.AsQueryable();
        
        if (noTracking)
        {
            query = query.AsNoTracking();
        }
        
        var domainFeedbackTags = await query
            .Where(ft => ft.TagId == tagId)
            .Include(ft => ft.Feedback)
            .Include(ft => ft.Tag)
            .ToListAsync();
            
        return domainFeedbackTags.Select(ft => Mapper.Map(ft)).OfType<App.DAL.DTO.FeedbackTag>();
    }
    
    public async Task<bool> FeedbackTagExistsAsync(Guid feedbackId, Guid tagId)
    {
        return await RepositoryDbSet
            .AnyAsync(ft => ft.FeedbackId == feedbackId && ft.TagId == tagId);
    }
    
    public async Task RemoveByFeedbackAndTagAsync(Guid feedbackId, Guid tagId)
    {
        var feedbackTag = await RepositoryDbSet
            .FirstOrDefaultAsync(ft => ft.FeedbackId == feedbackId && ft.TagId == tagId);
            
        if (feedbackTag != null)
        {
            RepositoryDbSet.Remove(feedbackTag);
        }
    }
}