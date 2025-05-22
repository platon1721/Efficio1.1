using App.BLL.Contracts;
using App.BLL.Mappers;
using App.DAL.Contracts;
using App.DAL.DTO;
using Base.BLL;
using Base.Contracts;

namespace App.BLL.Services;

public class TagService : BaseService<App.BLL.DTO.Tag, App.DAL.DTO.Tag, App.DAL.Contracts.ITagRepository>, ITagService
{

    public TagService(
        IAppUOW serviceUOW, 
        IMapper<App.BLL.DTO.Tag, App.DAL.DTO.Tag> mapper) : base(serviceUOW, serviceUOW.TagRepository, mapper)
    {
    }
    
    // public TagService(ITagRepository tagRepository, IMapper<App.BLL.DTO.Tag, App.DAL.DTO.Tag> mapper) 
    //     : base(tagRepository, mapper)
    // {
    //     _tagRepository = tagRepository;
    // }

    public async Task<IEnumerable<App.DAL.DTO.Tag>> GetTagsByTitleAsync(string title)
    {
        var dalTags = await ServiceRepository.GetTagsByTitleAsync(title);
        return dalTags;
    }

    public async Task<IEnumerable<App.DAL.DTO.Tag>> GetTagsByTitleContainsAsync(string searchTerm)
    {
        var dalTags = await ServiceRepository.GetTagsByTitleContainsAsync(searchTerm);
        return dalTags;
    }
}