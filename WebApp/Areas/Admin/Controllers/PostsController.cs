using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.DAL.EF;
using App.Domain;
using Microsoft.AspNetCore.Authorization;
using WebApp.Filters;

namespace WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    [ServiceFilter(typeof(InitializeCollectionsFilter))]
    public class PostsController : Controller
    {
        private readonly AppDbContext _context;

        public PostsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Posts
        public async Task<IActionResult> Index()
        {
            var posts = await _context.Posts
                .Include(p => p.PostTags!)
                .ThenInclude(pt => pt.Tag)
                .Include(p => p.PostDepartments!)
                .ThenInclude(pd => pd.Department)
                .Include(p => p.Comments!) // Include comments
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
            return View(posts);
        }

        // GET: Admin/Posts/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.PostTags!)
                .ThenInclude(pt => pt.Tag)
                .Include(p => p.PostDepartments!)
                .ThenInclude(pd => pd.Department)
                .Include(p => p.Comments!) // Include comments with full details
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (post == null)
            {
                return NotFound();
            }

            // Add comment statistics
            ViewBag.CommentStats = new
            {
                TotalComments = post.Comments?.Count ?? 0,
                RecentComments = post.Comments?.Count(c => c.CreatedAt >= DateTime.UtcNow.AddDays(-7)) ?? 0
            };

            return View(post);
        }

        // GET: Admin/Posts/Create
        public IActionResult Create()
        {
            ViewBag.Tags = new MultiSelectList(_context.Tags, "Id", "Title");
            ViewBag.Departments = new MultiSelectList(_context.Departments, "Id", "DepartmentName");
            return View();
        }

        // POST: Admin/Posts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description")] Post post, Guid[] selectedTags, Guid[] selectedDepartments)
        {
            ModelState.Remove("CreatedBy");
            ModelState.Remove("CreatedAt");
            ModelState.Remove("ChangedBy");
            ModelState.Remove("ChangedAt");
            
            if (ModelState.IsValid)
            {
                post.Id = Guid.NewGuid();
                _context.Add(post);
                
                // Add selected tags
                if (selectedTags != null && selectedTags.Length > 0)
                {
                    foreach (var tagId in selectedTags)
                    {
                        var postTag = new PostTag
                        {
                            Id = Guid.NewGuid(),
                            PostId = post.Id,
                            TagId = tagId
                        };
                        _context.PostTags.Add(postTag);
                    }
                }
                
                // Add selected departments
                if (selectedDepartments != null && selectedDepartments.Length > 0)
                {
                    foreach (var departmentId in selectedDepartments)
                    {
                        var postDepartment = new PostDepartment
                        {
                            Id = Guid.NewGuid(),
                            PostId = post.Id,
                            DepartmentId = departmentId
                        };
                        _context.PostDepartments.Add(postDepartment);
                    }
                }
                
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine($"ModelState Error: {modelError.ErrorMessage}");
            }
            
            ViewBag.Tags = new MultiSelectList(_context.Tags, "Id", "Title", selectedTags);
            ViewBag.Departments = new MultiSelectList(_context.Departments, "Id", "DepartmentName", selectedDepartments);
            return View(post);
        }
        
        // GET: Admin/Posts/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.PostTags!)
                .Include(p => p.PostDepartments!)
                .Include(p => p.Comments!) // Include comments for edit view
                .FirstOrDefaultAsync(p => p.Id == id);
            
            if (post == null)
            {
                return NotFound();
            }
            
            var selectedTagIds = post.PostTags?.Select(pt => pt.TagId).ToArray() ?? new Guid[0];
            var selectedDepartmentIds = post.PostDepartments?.Select(pd => pd.DepartmentId).ToArray() ?? new Guid[0];
            
            ViewBag.Tags = new MultiSelectList(_context.Tags, "Id", "Title", selectedTagIds);
            ViewBag.Departments = new MultiSelectList(_context.Departments, "Id", "DepartmentName", selectedDepartmentIds);
            
            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title,Description")] Post post, Guid[] selectedTags, Guid[] selectedDepartments)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            ModelState.Remove("CreatedBy");
            ModelState.Remove("CreatedAt");
            ModelState.Remove("ChangedBy");
            ModelState.Remove("ChangedAt");
            
            if (ModelState.IsValid)
            {
                try
                {
                    var existingPost = await _context.Posts
                        .Include(p => p.PostTags!)
                        .Include(p => p.PostDepartments!)
                        .FirstOrDefaultAsync(p => p.Id == id);
                    
                    if (existingPost == null)
                    {
                        return NotFound();
                    }
            
                    existingPost.Title = post.Title;
                    existingPost.Description = post.Description;
                    
                    // Update tags - remove existing and add new ones
                    if (existingPost.PostTags != null)
                    {
                        _context.PostTags.RemoveRange(existingPost.PostTags);
                    }
                    
                    if (selectedTags != null && selectedTags.Length > 0)
                    {
                        foreach (var tagId in selectedTags)
                        {
                            var postTag = new PostTag
                            {
                                Id = Guid.NewGuid(),
                                PostId = existingPost.Id,
                                TagId = tagId
                            };
                            _context.PostTags.Add(postTag);
                        }
                    }
                    
                    // Update departments - remove existing and add new ones
                    if (existingPost.PostDepartments != null)
                    {
                        _context.PostDepartments.RemoveRange(existingPost.PostDepartments);
                    }
                    
                    if (selectedDepartments != null && selectedDepartments.Length > 0)
                    {
                        foreach (var departmentId in selectedDepartments)
                        {
                            var postDepartment = new PostDepartment
                            {
                                Id = Guid.NewGuid(),
                                PostId = existingPost.Id,
                                DepartmentId = departmentId
                            };
                            _context.PostDepartments.Add(postDepartment);
                        }
                    }
            
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            
            ViewBag.Tags = new MultiSelectList(_context.Tags, "Id", "Title", selectedTags);
            ViewBag.Departments = new MultiSelectList(_context.Departments, "Id", "DepartmentName", selectedDepartments);
            return View(post);
        }

        // GET: Admin/Posts/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.PostTags!)
                .ThenInclude(pt => pt.Tag)
                .Include(p => p.PostDepartments!)
                .ThenInclude(pd => pd.Department)
                .Include(p => p.Comments!) // Include comments for delete confirmation
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Admin/Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var post = await _context.Posts
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post != null)
            {
                // Check if post has comments
                if (post.Comments?.Any() == true)
                {
                    TempData["Error"] = $"Cannot delete post '{post.Title}' because it has {post.Comments.Count} comment(s). Please delete the comments first.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Post '{post.Title}' has been deleted successfully.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Posts/AddComment/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(Guid postId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["Error"] = "Comment content cannot be empty.";
                return RedirectToAction(nameof(Details), new { id = postId });
            }

            if (content.Length > 1000)
            {
                TempData["Error"] = "Comment content cannot exceed 1000 characters.";
                return RedirectToAction(nameof(Details), new { id = postId });
            }

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                Content = content.Trim(),
                PostId = postId
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Comment added successfully.";
            return RedirectToAction(nameof(Details), new { id = postId });
        }

        private bool PostExists(Guid id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}