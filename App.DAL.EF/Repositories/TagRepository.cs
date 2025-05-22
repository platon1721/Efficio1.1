using App.DAL.Contracts;
using Base.DAL.EF;

namespace App.DAL.EF.Repositories;

public class TagRepository: BaseRepository<App.DAL.DTO.Tag, App.Domain.Tag>, ITagRepository
{
    public TagRepository(AppDbContext repositoryDbContext) : base(repositoryDbContext, new TagUOWMapper())
    {
    }
}