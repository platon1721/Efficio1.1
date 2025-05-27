using App.DAL.Contracts;
using App.DAL.EF.Mappers;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories;

public class FeedbackRepository : BaseRepository<App.DAL.DTO.Feedback, App.Domain.Feedback>, IFeedbackRepository
{
    public FeedbackRepository(AppDbContext repositoryDbContext) : base(repositoryDbContext, new FeedbackUOWMapper())
    {
    }

    public async Task<IEnumerable<App.DAL.DTO.Feedback>> GetAllWithTagsAndCommentsAsync(bool noTracking = true)
    {
        var query = RepositoryDbSet.AsQueryable();
        
        if (noTracking)
        {
            query = query.AsNoTracking();
        }
        
        var domainFeedbacks = await query
            .Include(f => f.FeedbackTags!)
                .ThenInclude(ft => ft.Tag)
            .Include(f => f.Comments)
            .Include(f => f.Department)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();
            
        return domainFeedbacks.Select(f => Mapper.Map(f)).OfType<App.DAL.DTO.Feedback>();
    }
    
    public async Task<App.DAL.DTO.Feedback?> GetWithTagsAndCommentsAsync(Guid id, bool noTracking = true)
    {
        var query = RepositoryDbSet.AsQueryable();
        
        if (noTracking)
        {
            query = query.AsNoTracking();
        }
        
        var domainFeedback = await query
            .Include(f => f.FeedbackTags!)
                .ThenInclude(ft => ft.Tag)
            .Include(f => f.Comments)
            .Include(f => f.Department)
            .FirstOrDefaultAsync(f => f.Id.Equals(id));
            
        return Mapper.Map(domainFeedback);
    }
    
    public async Task<IEnumerable<App.DAL.DTO.Feedback>> GetByDepartmentAsync(Guid departmentId, bool noTracking = true)
    {
        var query = RepositoryDbSet.AsQueryable();
        
        if (noTracking)
        {
            query = query.AsNoTracking();
        }
        
        var domainFeedbacks = await query
            .Where(f => f.DepartmentId == departmentId)
            .Include(f => f.FeedbackTags!)
                .ThenInclude(ft => ft.Tag)
            .Include(f => f.Comments)
            .Include(f => f.Department)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();
            
        return domainFeedbacks.Select(f => Mapper.Map(f)).OfType<App.DAL.DTO.Feedback>();
    }
    
    public async Task<IEnumerable<App.DAL.DTO.Feedback>> GetByTagAsync(Guid tagId, bool noTracking = true)
    {
        var query = RepositoryDbSet.AsQueryable();
        
        if (noTracking)
        {
            query = query.AsNoTracking();
        }
        
        var domainFeedbacks = await query
            .Where(f => f.FeedbackTags!.Any(ft => ft.TagId == tagId))
            .Include(f => f.FeedbackTags!)
                .ThenInclude(ft => ft.Tag)
            .Include(f => f.Comments)
            .Include(f => f.Department)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();
            
        return domainFeedbacks.Select(f => Mapper.Map(f)).OfType<App.DAL.DTO.Feedback>();
    }
}
