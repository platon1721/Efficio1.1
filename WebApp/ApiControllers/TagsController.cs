using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.BLL.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Base.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace WebApp.ApiControllers
{
    /// <summary>
    /// Tag management controller
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TagsController : ControllerBase
    {
        private readonly ILogger<TagsController> _logger;
        private readonly IAppBLL _bll;

        private readonly App.DTO.v1.Mappers.TagMapper _mapper =
            new App.DTO.v1.Mappers.TagMapper();

        /// <summary>
        /// Constructor
        /// </summary>
        public TagsController(IAppBLL bll, ILogger<TagsController> logger)
        {
            _bll = bll;
            _logger = logger;
        }

        /// <summary>
        /// Get all tags
        /// </summary>
        /// <returns>List of tags</returns>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<App.DTO.v1.Tag>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<App.DTO.v1.Tag>>> GetTags()
        {
            var data = await _bll.TagService.AllAsync();
            var res = data.Select(x => _mapper.Map(x)!).OrderBy(x => x.Title).ToList();
            return res;
        }

        /// <summary>
        /// Get tag by id
        /// </summary>
        /// <param name="id">Tag ID</param>
        /// <returns>Tag</returns>
        [HttpGet("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(App.DTO.v1.Tag), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<App.DTO.v1.Tag>> GetTag(Guid id)
        {
            var tag = await _bll.TagService.FindAsync(id);

            if (tag == null)
            {
                return NotFound();
            }

            return _mapper.Map(tag)!;
        }

        /// <summary>
        /// Update tag
        /// </summary>
        /// <param name="id">Tag ID</param>
        /// <param name="tag">Tag data</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutTag(Guid id, App.DTO.v1.Tag tag)
        {
            if (id != tag.Id)
            {
                return BadRequest();
            }

            var result = await _bll.TagService.UpdateAsync(_mapper.Map(tag)!);
            
            if (result == null)
            {
                return NotFound();
            }
            
            await _bll.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Create new tag
        /// </summary>
        /// <param name="tag">Tag creation data</param>
        /// <returns>Created tag</returns>
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(App.DTO.v1.Tag), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<App.DTO.v1.Tag>> PostTag(App.DTO.v1.CreateTag tag)
        {
            var bllEntity = _mapper.Map(tag);
            _bll.TagService.Add(bllEntity);
            await _bll.SaveChangesAsync();

            return CreatedAtAction("GetTag", new
            {
                id = bllEntity.Id,
                version = HttpContext.GetRequestedApiVersion()!.ToString()
            }, _mapper.Map(bllEntity)!);
        }

        /// <summary>
        /// Delete tag by id
        /// </summary>
        /// <param name="id">Tag ID</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTag(Guid id)
        {
            var tag = await _bll.TagService.FindAsync(id);
            if (tag == null)
            {
                return NotFound();
            }

            _bll.TagService.Remove(id);
            await _bll.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Search tags by title
        /// </summary>
        /// <param name="title">Title to search for</param>
        /// <returns>List of matching tags</returns>
        [HttpGet("search")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<App.DTO.v1.Tag>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<App.DTO.v1.Tag>>> SearchTags([FromQuery] string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return BadRequest("Title parameter is required");
            }

            var data = await _bll.TagService.GetTagsByTitleContainsAsync(title.Trim());
            var res = data.Select(x => _mapper.Map(x)!).OrderBy(x => x.Title).ToList();
            return res;
        }
    }
}
