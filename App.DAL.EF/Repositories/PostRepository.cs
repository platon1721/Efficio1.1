using App.DAL.Contracts;
using App.DAL.EF.Mappers;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories;

public class PostRepository : BaseRepository<App.DAL.DTO.Post, App.Domain.Post>, IPostRepository
{
    public PostRepository(AppDbContext repositoryDbContext) : base(repositoryDbContext, new PostUOWMapper())
    {
    }

    public async Task<IEnumerable<App.DAL.DTO.Post>> GetAllWithTagsAndDepartmentsAsync(bool noTracking = true)
    {
        var query = RepositoryDbSet.AsQueryable();
        
        if (noTracking)
        {
            query = query.AsNoTracking();
        }
        
        var domainPosts = await query
            .Include(p => p.PostTags!)
            .ThenInclude(pt => pt.Tag)
            .Include(p => p.PostDepartments!)
            .ThenInclude(pd => pd.Department)
            .ToListAsync();
            
        return domainPosts.Select(p => Mapper.Map(p)).OfType<App.DAL.DTO.Post>();
    }
    
    public async Task<App.DAL.DTO.Post?> GetWithTagsAndDepartmentsAsync(Guid id, bool noTracking = true)
    {
        var query = RepositoryDbSet.AsQueryable();
        
        if (noTracking)
        {
            query = query.AsNoTracking();
        }
        
        var domainPost = await query
            .Include(p => p.PostTags!)
                .ThenInclude(pt => pt.Tag)
            .Include(p => p.PostDepartments!)
                .ThenInclude(pd => pd.Department)
            .FirstOrDefaultAsync(p => p.Id.Equals(id));
            
        return Mapper.Map(domainPost);
    }
    
    public async Task<IEnumerable<App.DAL.DTO.Post>> GetByTagAsync(Guid tagId, bool noTracking = true)
    {
        var query = RepositoryDbSet.AsQueryable();
        
        if (noTracking)
        {
            query = query.AsNoTracking();
        }
        
        var domainPosts = await query
            .Where(p => p.PostTags!.Any(pt => pt.TagId == tagId))
            .Include(p => p.PostTags!)
                .ThenInclude(pt => pt.Tag)
            .Include(p => p.PostDepartments!)
                .ThenInclude(pd => pd.Department)
            .ToListAsync();
            
        return domainPosts.Select(p => Mapper.Map(p)).OfType<App.DAL.DTO.Post>();
    }
    
    public async Task<IEnumerable<App.DAL.DTO.Post>> GetByDepartmentAsync(Guid departmentId, bool noTracking = true)
    {
        var query = RepositoryDbSet.AsQueryable();
        
        if (noTracking)
        {
            query = query.AsNoTracking();
        }
        
        var domainPosts = await query
            .Where(p => p.PostDepartments!.Any(pd => pd.DepartmentId == departmentId))
            .Include(p => p.PostTags!)
                .ThenInclude(pt => pt.Tag)
            .Include(p => p.PostDepartments!)
                .ThenInclude(pd => pd.Department)
            .ToListAsync();
            
        return domainPosts.Select(p => Mapper.Map(p)).OfType<App.DAL.DTO.Post>();
    }
}
