// WebApp/ApiControllers/PostsController.cs - Proper implementation
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
    /// Post management controller
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PostsController : ControllerBase
    {
        private readonly ILogger<PostsController> _logger;
        private readonly IAppBLL _bll;

        private readonly App.DTO.v1.Mappers.PostMapper _mapper =
            new App.DTO.v1.Mappers.PostMapper();

        /// <summary>
        /// Constructor
        /// </summary>
        public PostsController(IAppBLL bll, ILogger<PostsController> logger)
        {
            _bll = bll;
            _logger = logger;
        }

        /// <summary>
        /// Get all posts with tags, departments and comments
        /// </summary>
        /// <returns>List of posts</returns>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<App.DTO.v1.Post>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<App.DTO.v1.Post>>> GetPosts()
        {
            var data = await _bll.PostService.GetAllWithTagsDepartmentsAndCommentsAsync();
            var res = data.Select(x => _mapper.Map(x)!).OrderByDescending(x => x.Id).ToList();
            return res;
        }

        /// <summary>
        /// Get post by id with full relations
        /// </summary>
        /// <param name="id">Post ID</param>
        /// <returns>Post with relations</returns>
        [HttpGet("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(App.DTO.v1.Post), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<App.DTO.v1.Post>> GetPost(Guid id)
        {
            var post = await _bll.PostService.GetWithTagsDepartmentsAndCommentsAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            return _mapper.Map(post)!;
        }

        /// <summary>
        /// Update post
        /// </summary>
        /// <param name="id">Post ID</param>
        /// <param name="post">Post data</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutPost(Guid id, App.DTO.v1.Post post)
        {
            if (id != post.Id)
            {
                return BadRequest();
            }

            var result = await _bll.PostService.UpdateAsync(_mapper.Map(post)!);
            
            if (result == null)
            {
                return NotFound();
            }
            
            await _bll.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Create new post
        /// </summary>
        /// <param name="post">Post creation data</param>
        /// <returns>Created post</returns>
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(App.DTO.v1.Post), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<App.DTO.v1.Post>> PostPost(App.DTO.v1.CreatePost post)
        {
            var bllEntity = _mapper.Map(post);
            _bll.PostService.Add(bllEntity);
            await _bll.SaveChangesAsync();

            return CreatedAtAction("GetPost", new
            {
                id = bllEntity.Id,
                version = HttpContext.GetRequestedApiVersion()!.ToString()
            }, _mapper.Map(bllEntity)!);
        }

        /// <summary>
        /// Delete post by id
        /// </summary>
        /// <param name="id">Post ID</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            var post = await _bll.PostService.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            _bll.PostService.Remove(id);
            await _bll.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Get posts by tag
        /// </summary>
        /// <param name="tagId">Tag ID</param>
        /// <returns>List of posts with this tag</returns>
        [HttpGet("tag/{tagId}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<App.DTO.v1.Post>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<App.DTO.v1.Post>>> GetPostsByTag(Guid tagId)
        {
            var data = await _bll.PostService.GetByTagAsync(tagId);
            var res = data.Select(x => _mapper.Map(x)!).OrderByDescending(x => x.Id).ToList();
            return res;
        }

        /// <summary>
        /// Get posts by department
        /// </summary>
        /// <param name="departmentId">Department ID</param>
        /// <returns>List of posts for this department</returns>
        [HttpGet("department/{departmentId}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<App.DTO.v1.Post>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<App.DTO.v1.Post>>> GetPostsByDepartment(Guid departmentId)
        {
            var data = await _bll.PostService.GetByDepartmentAsync(departmentId);
            var res = data.Select(x => _mapper.Map(x)!).OrderByDescending(x => x.Id).ToList();
            return res;
        }

        /// <summary>
        /// Add tag to post
        /// </summary>
        /// <param name="id">Post ID</param>
        /// <param name="tagId">Tag ID</param>
        /// <returns></returns>
        [HttpPost("{id}/tags/{tagId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddTagToPost(Guid id, Guid tagId)
        {
            var success = await _bll.PostService.AddTagToPostAsync(id, tagId);
            
            if (!success)
            {
                return BadRequest("Tag already exists on post or post/tag not found");
            }

            await _bll.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Remove tag from post
        /// </summary>
        /// <param name="id">Post ID</param>
        /// <param name="tagId">Tag ID</param>
        /// <returns></returns>
        [HttpDelete("{id}/tags/{tagId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveTagFromPost(Guid id, Guid tagId)
        {
            var success = await _bll.PostService.RemoveTagFromPostAsync(id, tagId);
            
            if (!success)
            {
                return NotFound("Tag not found on post");
            }

            await _bll.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Add department to post
        /// </summary>
        /// <param name="id">Post ID</param>
        /// <param name="departmentId">Department ID</param>
        /// <returns></returns>
        [HttpPost("{id}/departments/{departmentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddDepartmentToPost(Guid id, Guid departmentId)
        {
            var success = await _bll.PostService.AddDepartmentToPostAsync(id, departmentId);
            
            if (!success)
            {
                return BadRequest("Department already exists on post or post/department not found");
            }

            await _bll.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Remove department from post
        /// </summary>
        /// <param name="id">Post ID</param>
        /// <param name="departmentId">Department ID</param>
        /// <returns></returns>
        [HttpDelete("{id}/departments/{departmentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveDepartmentFromPost(Guid id, Guid departmentId)
        {
            var success = await _bll.PostService.RemoveDepartmentFromPostAsync(id, departmentId);
            
            if (!success)
            {
                return NotFound("Department not found on post");
            }

            await _bll.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Get post comments
        /// </summary>
        /// <param name="id">Post ID</param>
        /// <returns>List of comments</returns>
        [HttpGet("{id}/comments")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<App.DTO.v1.Comment>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<App.DTO.v1.Comment>>> GetPostComments(Guid id)
        {
            var post = await _bll.PostService.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            var commentMapper = new App.DTO.v1.Mappers.CommentMapper();
            var comments = await _bll.PostService.GetPostCommentsAsync(id);
            var res = comments.Select(x => commentMapper.Map(x)!).OrderBy(x => x.CreatedAt).ToList();
            return res;
        }

        /// <summary>
        /// Add comment to post
        /// </summary>
        /// <param name="id">Post ID</param>
        /// <param name="comment">Comment creation data</param>
        /// <returns>Created comment</returns>
        [HttpPost("{id}/comments")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(App.DTO.v1.Comment), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<App.DTO.v1.Comment>> AddCommentToPost(Guid id, [FromBody] App.DTO.v1.CreateComment comment)
        {
            if (comment.PostId != id)
            {
                comment.PostId = id; // Ensure consistency
            }

            if (comment.FeedbackId.HasValue)
            {
                return BadRequest("Comment cannot belong to both post and feedback");
            }

            try
            {
                var commentMapper = new App.DTO.v1.Mappers.CommentMapper();
                var result = await _bll.PostService.AddCommentToPostAsync(id, comment.Content);
                
                if (result == null)
                {
                    return NotFound("Post not found");
                }

                return CreatedAtAction("GetPostComments", new { id = id }, commentMapper.Map(result)!);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Remove comment from post  
        /// </summary>
        /// <param name="id">Post ID</param>
        /// <param name="commentId">Comment ID</param>
        /// <returns></returns>
        [HttpDelete("{id}/comments/{commentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveCommentFromPost(Guid id, Guid commentId)
        {
            var success = await _bll.PostService.RemoveCommentFromPostAsync(id, commentId);
            
            if (!success)
            {
                return NotFound("Comment not found or does not belong to this post");
            }

            return Ok();
        }

        /// <summary>
        /// Get comment count for post
        /// </summary>
        /// <param name="id">Post ID</param>
        /// <returns>Comment count</returns>
        [HttpGet("{id}/comments/count")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<int>> GetPostCommentCount(Guid id)
        {
            var post = await _bll.PostService.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            var count = await _bll.PostService.GetPostCommentCountAsync(id);
            return count;
        }

        /// <summary>
        /// Update post tags in bulk
        /// </summary>
        /// <param name="id">Post ID</param>
        /// <param name="tagIds">List of tag IDs</param>
        /// <returns></returns>
        [HttpPut("{id}/tags")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePostTags(Guid id, [FromBody] IEnumerable<Guid> tagIds)
        {
            var success = await _bll.PostService.UpdatePostTagsAsync(id, tagIds);
            
            if (!success)
            {
                return NotFound("Post not found");
            }

            return Ok();
        }

        /// <summary>
        /// Update post departments in bulk
        /// </summary>
        /// <param name="id">Post ID</param>
        /// <param name="departmentIds">List of department IDs</param>
        /// <returns></returns>
        [HttpPut("{id}/departments")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePostDepartments(Guid id, [FromBody] IEnumerable<Guid> departmentIds)
        {
            var success = await _bll.PostService.UpdatePostDepartmentsAsync(id, departmentIds);
            
            if (!success)
            {
                return NotFound("Post not found");
            }

            return Ok();
        }
    }
}