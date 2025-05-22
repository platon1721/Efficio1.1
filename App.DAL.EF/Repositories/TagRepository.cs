using App.DAL.Contracts;
using App.DAL.EF.Mappers;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories;

public class TagRepository: BaseRepository<App.DAL.DTO.Tag, App.Domain.Tag>, ITagRepository
{
    public TagRepository(AppDbContext repositoryDbContext) : base(repositoryDbContext, new TagUOWMapper())
    {
    }

    public async Task<IEnumerable<App.DAL.DTO.Tag>> GetTagsByTitleContainsAsync(string searchTerm)
    {
        var domainTags = await RepositoryDbSet
            .Where(t => t.Title.ToLower()
                .Contains(searchTerm.ToLower()))
            .OrderBy(t => t.Title)
            .ToListAsync();
            
        return domainTags.Select(t => Mapper.Map(t)).OfType<App.DAL.DTO.Tag>();
    }
    
    // Search for tags by title (backward compatibility)
    public async Task<IEnumerable<App.DAL.DTO.Tag>> GetTagsByTitleAsync(string title)
    {
        return await GetTagsByTitleContainsAsync(title);
    }

}