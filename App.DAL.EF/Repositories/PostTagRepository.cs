using App.DAL.Contracts;
using App.DAL.EF.Mappers;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories;

public class PostTagRepository : BaseRepository<App.DAL.DTO.PostTag, App.Domain.PostTag>, IPostTagRepository
{
    public PostTagRepository(AppDbContext repositoryDbContext) : base(repositoryDbContext, new PostTagUOWMapper())
    {
    }

    public async Task<IEnumerable<App.DAL.DTO.PostTag>> GetByPostIdAsync(Guid postId, bool noTracking = true)
    {
        var query = RepositoryDbSet.AsQueryable();
        
        if (noTracking)
        {
            query = query.AsNoTracking();
        }
        
        var domainPostTags = await query
            .Where(pt => pt.PostId == postId)
            .Include(pt => pt.Post)
            .Include(pt => pt.Tag)
            .ToListAsync();
            
        return domainPostTags.Select(pt => Mapper.Map(pt)).OfType<App.DAL.DTO.PostTag>();
    }
    
    public async Task<IEnumerable<App.DAL.DTO.PostTag>> GetByTagIdAsync(Guid tagId, bool noTracking = true)
    {
        var query = RepositoryDbSet.AsQueryable();
        
        if (noTracking)
        {
            query = query.AsNoTracking();
        }
        
        var domainPostTags = await query
            .Where(pt => pt.TagId == tagId)
            .Include(pt => pt.Post)
            .Include(pt => pt.Tag)
            .ToListAsync();
            
        return domainPostTags.Select(pt => Mapper.Map(pt)).OfType<App.DAL.DTO.PostTag>();
    }
    
    public async Task<bool> PostTagExistsAsync(Guid postId, Guid tagId)
    {
        return await RepositoryDbSet
            .AnyAsync(pt => pt.PostId == postId && pt.TagId == tagId);
    }
    
    public async Task RemoveByPostAndTagAsync(Guid postId, Guid tagId)
    {
        var postTag = await RepositoryDbSet
            .FirstOrDefaultAsync(pt => pt.PostId == postId && pt.TagId == tagId);
            
        if (postTag != null)
        {
            RepositoryDbSet.Remove(postTag);
        }
    }
}