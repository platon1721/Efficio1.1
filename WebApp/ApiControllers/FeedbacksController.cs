// WebApp/ApiControllers/FeedbacksController.cs - Proper implementation
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
    /// Feedback management controller
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FeedbacksController : ControllerBase
    {
        private readonly ILogger<FeedbacksController> _logger;
        private readonly IAppBLL _bll;

        private readonly App.DTO.v1.Mappers.FeedbackMapper _mapper =
            new App.DTO.v1.Mappers.FeedbackMapper();

        /// <summary>
        /// Constructor
        /// </summary>
        public FeedbacksController(IAppBLL bll, ILogger<FeedbacksController> logger)
        {
            _bll = bll;
            _logger = logger;
        }

        /// <summary>
        /// Get all feedbacks with tags and comments
        /// </summary>
        /// <returns>List of feedbacks</returns>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<App.DTO.v1.Feedback>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<App.DTO.v1.Feedback>>> GetFeedbacks()
        {
            var data = await _bll.FeedbackService.GetAllWithTagsAndCommentsAsync();
            var res = data.Select(x => _mapper.Map(x)!).OrderByDescending(x => x.Id).ToList();
            return res;
        }

        /// <summary>
        /// Get feedback by id with full relations
        /// </summary>
        /// <param name="id">Feedback ID</param>
        /// <returns>Feedback with relations</returns>
        [HttpGet("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(App.DTO.v1.Feedback), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<App.DTO.v1.Feedback>> GetFeedback(Guid id)
        {
            var feedback = await _bll.FeedbackService.GetWithTagsAndCommentsAsync(id);

            if (feedback == null)
            {
                return NotFound();
            }

            return _mapper.Map(feedback)!;
        }

        /// <summary>
        /// Update feedback
        /// </summary>
        /// <param name="id">Feedback ID</param>
        /// <param name="feedback">Feedback data</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutFeedback(Guid id, App.DTO.v1.Feedback feedback)
        {
            if (id != feedback.Id)
            {
                return BadRequest();
            }

            var result = await _bll.FeedbackService.UpdateAsync(_mapper.Map(feedback)!);
            
            if (result == null)
            {
                return NotFound();
            }
            
            await _bll.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Create new feedback
        /// </summary>
        /// <param name="feedback">Feedback creation data</param>
        /// <returns>Created feedback</returns>
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(App.DTO.v1.Feedback), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<App.DTO.v1.Feedback>> PostFeedback(App.DTO.v1.CreateFeedback feedback)
        {
            var bllEntity = _mapper.Map(feedback);
            
            // If tags are provided, create feedback with tags
            if (feedback.TagIds != null && feedback.TagIds.Any())
            {
                var result = await _bll.FeedbackService.CreateFeedbackWithTagsAsync(bllEntity, feedback.TagIds);
                return CreatedAtAction("GetFeedback", new
                {
                    id = result!.Id,
                    version = HttpContext.GetRequestedApiVersion()!.ToString()
                }, _mapper.Map(result)!);
            }
            else
            {
                _bll.FeedbackService.Add(bllEntity);
                await _bll.SaveChangesAsync();

                return CreatedAtAction("GetFeedback", new
                {
                    id = bllEntity.Id,
                    version = HttpContext.GetRequestedApiVersion()!.ToString()
                }, _mapper.Map(bllEntity)!);
            }
        }

        /// <summary>
        /// Delete feedback by id
        /// </summary>
        /// <param name="id">Feedback ID</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteFeedback(Guid id)
        {
            var feedback = await _bll.FeedbackService.FindAsync(id);
            if (feedback == null)
            {
                return NotFound();
            }

            _bll.FeedbackService.Remove(id);
            await _bll.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Get feedbacks by department
        /// </summary>
        /// <param name="departmentId">Department ID</param>
        /// <returns>List of department feedbacks</returns>
        [HttpGet("department/{departmentId}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<App.DTO.v1.Feedback>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<App.DTO.v1.Feedback>>> GetFeedbacksByDepartment(Guid departmentId)
        {
            var data = await _bll.FeedbackService.GetByDepartmentAsync(departmentId);
            var res = data.Select(x => _mapper.Map(x)!).OrderByDescending(x => x.Id).ToList();
            return res;
        }

        /// <summary>
        /// Get feedbacks by tag
        /// </summary>
        /// <param name="tagId">Tag ID</param>
        /// <returns>List of feedbacks with this tag</returns>
        [HttpGet("tag/{tagId}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<App.DTO.v1.Feedback>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<App.DTO.v1.Feedback>>> GetFeedbacksByTag(Guid tagId)
        {
            var data = await _bll.FeedbackService.GetByTagAsync(tagId);
            var res = data.Select(x => _mapper.Map(x)!).OrderByDescending(x => x.Id).ToList();
            return res;
        }

        /// <summary>
        /// Add tag to feedback
        /// </summary>
        /// <param name="id">Feedback ID</param>
        /// <param name="tagId">Tag ID</param>
        /// <returns></returns>
        [HttpPost("{id}/tags/{tagId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddTagToFeedback(Guid id, Guid tagId)
        {
            var success = await _bll.FeedbackService.AddTagToFeedbackAsync(id, tagId);
            
            if (!success)
            {
                return BadRequest("Tag already exists on feedback or feedback/tag not found");
            }

            await _bll.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Remove tag from feedback
        /// </summary>
        /// <param name="id">Feedback ID</param>
        /// <param name="tagId">Tag ID</param>
        /// <returns></returns>
        [HttpDelete("{id}/tags/{tagId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveTagFromFeedback(Guid id, Guid tagId)
        {
            var success = await _bll.FeedbackService.RemoveTagFromFeedbackAsync(id, tagId);
            
            if (!success)
            {
                return NotFound("Tag not found on feedback");
            }

            await _bll.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Update feedback tags in bulk
        /// </summary>
        /// <param name="id">Feedback ID</param>
        /// <param name="tagIds">List of tag IDs</param>
        /// <returns></returns>
        [HttpPut("{id}/tags")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateFeedbackTags(Guid id, [FromBody] IEnumerable<Guid> tagIds)
        {
            var success = await _bll.FeedbackService.UpdateFeedbackTagsAsync(id, tagIds);
            
            if (!success)
            {
                return NotFound("Feedback not found");
            }

            return Ok();
        }

        /// <summary>
        /// Get feedback comments
        /// </summary>
        /// <param name="id">Feedback ID</param>
        /// <returns>List of comments</returns>
        [HttpGet("{id}/comments")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<App.DTO.v1.Comment>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<App.DTO.v1.Comment>>> GetFeedbackComments(Guid id)
        {
            var feedback = await _bll.FeedbackService.FindAsync(id);
            if (feedback == null)
            {
                return NotFound();
            }

            var commentMapper = new App.DTO.v1.Mappers.CommentMapper();
            var comments = await _bll.CommentService.GetByFeedbackIdAsync(id);
            var res = comments.Select(x => commentMapper.Map(x)!).OrderBy(x => x.CreatedAt).ToList();
            return res;
        }

        /// <summary>
        /// Add comment to feedback
        /// </summary>
        /// <param name="id">Feedback ID</param>
        /// <param name="comment">Comment creation data</param>
        /// <returns>Created comment</returns>
        [HttpPost("{id}/comments")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(App.DTO.v1.Comment), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<App.DTO.v1.Comment>> AddCommentToFeedback(Guid id, [FromBody] App.DTO.v1.CreateComment comment)
        {
            if (comment.FeedbackId != id)
            {
                comment.FeedbackId = id; // Ensure consistency
            }

            if (comment.PostId.HasValue)
            {
                return BadRequest("Comment cannot belong to both post and feedback");
            }

            try
            {
                var commentMapper = new App.DTO.v1.Mappers.CommentMapper();
                var result = await _bll.CommentService.AddCommentToFeedbackAsync(id, comment.Content);
                
                if (result == null)
                {
                    return NotFound("Feedback not found");
                }

                return CreatedAtAction("GetFeedbackComments", new { id = id }, commentMapper.Map(result)!);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}