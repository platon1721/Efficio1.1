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
    /// Comment management controller
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CommentsController : ControllerBase
    {
        private readonly ILogger<CommentsController> _logger;
        private readonly IAppBLL _bll;

        private readonly App.DTO.v1.Mappers.CommentMapper _mapper =
            new App.DTO.v1.Mappers.CommentMapper();

        /// <summary>
        /// Constructor
        /// </summary>
        public CommentsController(IAppBLL bll, ILogger<CommentsController> logger)
        {
            _bll = bll;
            _logger = logger;
        }

        /// <summary>
        /// Get all comments
        /// </summary>
        /// <returns>List of comments</returns>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<App.DTO.v1.Comment>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<App.DTO.v1.Comment>>> GetComments()
        {
            var data = await _bll.CommentService.AllAsync();
            var res = data.Select(x => _mapper.Map(x)!).OrderByDescending(x => x.Id).ToList();
            return res;
        }

        /// <summary>
        /// Get comment by id
        /// </summary>
        /// <param name="id">Comment ID</param>
        /// <returns>Comment</returns>
        [HttpGet("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(App.DTO.v1.Comment), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<App.DTO.v1.Comment>> GetComment(Guid id)
        {
            var comment = await _bll.CommentService.FindAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            return _mapper.Map(comment)!;
        }

        /// <summary>
        /// Update comment
        /// </summary>
        /// <param name="id">Comment ID</param>
        /// <param name="comment">Comment data</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutComment(Guid id, App.DTO.v1.Comment comment)
        {
            if (id != comment.Id)
            {
                return BadRequest();
            }

            var result = await _bll.CommentService.UpdateAsync(_mapper.Map(comment)!);
            
            if (result == null)
            {
                return NotFound();
            }
            
            await _bll.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Create new comment
        /// </summary>
        /// <param name="comment">Comment creation data</param>
        /// <returns>Created comment</returns>
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(App.DTO.v1.Comment), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<App.DTO.v1.Comment>> PostComment(App.DTO.v1.CreateComment comment)
        {
            try
            {
                var bllEntity = _mapper.Map(comment);
                
                // Validate business rules
                if (!await _bll.CommentService.ValidateCommentAsync(bllEntity))
                {
                    return BadRequest("Comment must belong to either a post or feedback, but not both");
                }

                _bll.CommentService.Add(bllEntity);
                await _bll.SaveChangesAsync();

                return CreatedAtAction("GetComment", new
                {
                    id = bllEntity.Id,
                    version = HttpContext.GetRequestedApiVersion()!.ToString()
                }, _mapper.Map(bllEntity)!);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Delete comment by id
        /// </summary>
        /// <param name="id">Comment ID</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            var comment = await _bll.CommentService.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            _bll.CommentService.Remove(id);
            await _bll.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Get comments by post
        /// </summary>
        /// <param name="postId">Post ID</param>
        /// <returns>List of post comments</returns>
        [HttpGet("post/{postId}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<App.DTO.v1.Comment>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<App.DTO.v1.Comment>>> GetCommentsByPost(Guid postId)
        {
            var data = await _bll.CommentService.GetByPostIdAsync(postId);
            var res = data.Select(x => _mapper.Map(x)!).OrderBy(x => x.CreatedAt).ToList();
            return res;
        }

        /// <summary>
        /// Get comments by feedback
        /// </summary>
        /// <param name="feedbackId">Feedback ID</param>
        /// <returns>List of feedback comments</returns>
        [HttpGet("feedback/{feedbackId}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<App.DTO.v1.Comment>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<App.DTO.v1.Comment>>> GetCommentsByFeedback(Guid feedbackId)
        {
            var data = await _bll.CommentService.GetByFeedbackIdAsync(feedbackId);
            var res = data.Select(x => _mapper.Map(x)!).OrderBy(x => x.CreatedAt).ToList();
            return res;
        }
    }
}