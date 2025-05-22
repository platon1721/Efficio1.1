using App.DAL.Contracts;
using App.DAL.EF.Mappers;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories;

public class PostDepartmentRepository : BaseRepository<App.DAL.DTO.PostDepartment, App.Domain.PostDepartment>, IPostDepartmentRepository
{
    public PostDepartmentRepository(AppDbContext repositoryDbContext) : base(repositoryDbContext, new PostDepartmentUOWMapper())
    {
    }

    public async Task<IEnumerable<App.DAL.DTO.PostDepartment>> GetByPostIdAsync(Guid postId, bool noTracking = true)
    {
        var query = RepositoryDbSet.AsQueryable();
        
        if (noTracking)
        {
            query = query.AsNoTracking();
        }
        
        var domainPostDepartments = await query
            .Where(pd => pd.PostId == postId)
            .Include(pd => pd.Post)
            .Include(pd => pd.Department)
            .ToListAsync();
            
        return domainPostDepartments.Select(pd => Mapper.Map(pd)).OfType<App.DAL.DTO.PostDepartment>();
    }
    
    public async Task<IEnumerable<App.DAL.DTO.PostDepartment>> GetByDepartmentIdAsync(Guid departmentId, bool noTracking = true)
    {
        var query = RepositoryDbSet.AsQueryable();
        
        if (noTracking)
        {
            query = query.AsNoTracking();
        }
        
        var domainPostDepartments = await query
            .Where(pd => pd.DepartmentId == departmentId)
            .Include(pd => pd.Post)
            .Include(pd => pd.Department)
            .ToListAsync();
            
        return domainPostDepartments.Select(pd => Mapper.Map(pd)).OfType<App.DAL.DTO.PostDepartment>();
    }
    
    public async Task<bool> PostDepartmentExistsAsync(Guid postId, Guid departmentId)
    {
        return await RepositoryDbSet
            .AnyAsync(pd => pd.PostId == postId && pd.DepartmentId == departmentId);
    }
    
    public async Task RemoveByPostAndDepartmentAsync(Guid postId, Guid departmentId)
    {
        var postDepartment = await RepositoryDbSet
            .FirstOrDefaultAsync(pd => pd.PostId == postId && pd.DepartmentId == departmentId);
            
        if (postDepartment != null)
        {
            RepositoryDbSet.Remove(postDepartment);
        }
    }
}