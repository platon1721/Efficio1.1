using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.DAL.EF;
using App.Domain;
using Microsoft.AspNetCore.Mvc;
using Task = System.Threading.Tasks.Task;

namespace WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CommentsController : Controller
    {
        private readonly AppDbContext _context;

        public CommentsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Comments
        public async Task<IActionResult> Index()
        {
            var comments = await _context.Comments
                .Include(c => c.Feedback)
                .Include(c => c.Post)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return View(comments);
        }

        // GET: Admin/Comments/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.Feedback)
                .ThenInclude(f => f!.Department)
                .Include(c => c.Post)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // GET: Admin/Comments/Create
        public async Task<IActionResult> Create(Guid? postId, Guid? feedbackId)
        {
            // Validate that only one parent is specified
            if (postId.HasValue && feedbackId.HasValue)
            {
                TempData["Error"] = "A comment cannot belong to both a Post and Feedback.";
                return RedirectToAction("Index");
            }

            await PopulateDropdowns(postId, feedbackId);

            var comment = new Comment();
            if (postId.HasValue)
            {
                comment.PostId = postId.Value;

                // Get post title for display
                var post = await _context.Posts.FindAsync(postId.Value);
                ViewBag.ParentTitle = post?.Title ?? "Unknown Post";
                ViewBag.ParentType = "Post";
            }
            else if (feedbackId.HasValue)
            {
                comment.FeedbackId = feedbackId.Value;

                // Get feedback title for display
                var feedback = await _context.Feedbacks.FindAsync(feedbackId.Value);
                ViewBag.ParentTitle = feedback?.Title ?? "Unknown Feedback";
                ViewBag.ParentType = "Feedback";
            }

            return View(comment);
        }

        // POST: Admin/Comments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Content,PostId,FeedbackId")] Comment comment)
        {
            // Custom validation: ensure comment belongs to either Post OR Feedback, but not both
            if (comment.PostId.HasValue && comment.FeedbackId.HasValue)
            {
                ModelState.AddModelError("",
                    "A comment cannot belong to both a Post and Feedback. Please select only one.");
            }
            else if (!comment.PostId.HasValue && !comment.FeedbackId.HasValue)
            {
                ModelState.AddModelError("", "A comment must belong to either a Post or Feedback. Please select one.");
            }

            // Validate that the referenced Post or Feedback exists
            if (comment.PostId.HasValue)
            {
                var postExists = await _context.Posts.AnyAsync(p => p.Id == comment.PostId.Value);
                if (!postExists)
                {
                    ModelState.AddModelError("PostId", "The selected Post does not exist.");
                }
            }

            if (comment.FeedbackId.HasValue)
            {
                var feedbackExists = await _context.Feedbacks.AnyAsync(f => f.Id == comment.FeedbackId.Value);
                if (!feedbackExists)
                {
                    ModelState.AddModelError("FeedbackId", "The selected Feedback does not exist.");
                }
            }

            if (ModelState.IsValid)
            {
                comment.Id = Guid.NewGuid();
                _context.Add(comment);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Comment has been created successfully.";

                // Redirect to the parent entity's details page
                if (comment.PostId.HasValue)
                {
                    return RedirectToAction("Details", "Posts", new { id = comment.PostId });
                }
                else if (comment.FeedbackId.HasValue)
                {
                    return RedirectToAction("Details", "Feedbacks", new { id = comment.FeedbackId });
                }

                return RedirectToAction(nameof(Index));
            }

            await PopulateDropdowns(comment.PostId, comment.FeedbackId);
            return View(comment);
        }

        // GET: Admin/Comments/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.Post)
                .Include(c => c.Feedback)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (comment == null)
            {
                return NotFound();
            }

            await PopulateDropdowns(comment.PostId, comment.FeedbackId);

            // Set parent info for display
            if (comment.PostId.HasValue)
            {
                ViewBag.ParentTitle = comment.Post?.Title ?? "Unknown Post";
                ViewBag.ParentType = "Post";
            }
            else if (comment.FeedbackId.HasValue)
            {
                ViewBag.ParentTitle = comment.Feedback?.Title ?? "Unknown Feedback";
                ViewBag.ParentType = "Feedback";
            }

            return View(comment);
        }

        // POST: Admin/Comments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Content,PostId,FeedbackId")] Comment comment)
        {
            if (id != comment.Id)
            {
                return NotFound();
            }

            // Custom validation: ensure comment belongs to either Post OR Feedback, but not both
            if (comment.PostId.HasValue && comment.FeedbackId.HasValue)
            {
                ModelState.AddModelError("",
                    "A comment cannot belong to both a Post and Feedback. Please select only one.");
            }
            else if (!comment.PostId.HasValue && !comment.FeedbackId.HasValue)
            {
                ModelState.AddModelError("", "A comment must belong to either a Post or Feedback. Please select one.");
            }

            // Validate that the referenced Post or Feedback exists
            if (comment.PostId.HasValue)
            {
                var postExists = await _context.Posts.AnyAsync(p => p.Id == comment.PostId.Value);
                if (!postExists)
                {
                    ModelState.AddModelError("PostId", "The selected Post does not exist.");
                }
            }

            if (comment.FeedbackId.HasValue)
            {
                var feedbackExists = await _context.Feedbacks.AnyAsync(f => f.Id == comment.FeedbackId.Value);
                if (!feedbackExists)
                {
                    ModelState.AddModelError("FeedbackId", "The selected Feedback does not exist.");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(comment);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Comment has been updated successfully.";

                    // Redirect to the parent entity's details page
                    if (comment.PostId.HasValue)
                    {
                        return RedirectToAction("Details", "Posts", new { id = comment.PostId });
                    }
                    else if (comment.FeedbackId.HasValue)
                    {
                        return RedirectToAction("Details", "Feedbacks", new { id = comment.FeedbackId });
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            await PopulateDropdowns(comment.PostId, comment.FeedbackId);
            return View(comment);
        }

        // GET: Admin/Comments/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.Feedback)
                .ThenInclude(f => f!.Department)
                .Include(c => c.Post)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Admin/Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var comment = await _context.Comments
                .Include(c => c.Post)
                .Include(c => c.Feedback)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (comment != null)
            {
                var parentId = comment.PostId ?? comment.FeedbackId;
                var parentType = comment.PostId.HasValue ? "Post" : "Feedback";
                var parentController = comment.PostId.HasValue ? "Posts" : "Feedbacks";

                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Comment has been deleted successfully.";

                // Redirect back to parent entity if we know it
                if (parentId.HasValue)
                {
                    return RedirectToAction("Details", parentController, new { id = parentId });
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Comments/QuickAdd
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuickAdd(Guid? postId, Guid? feedbackId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["Error"] = "Comment content cannot be empty.";
            }
            else if (content.Length > 1000)
            {
                TempData["Error"] = "Comment content cannot exceed 1000 characters.";
            }
            else if (!postId.HasValue && !feedbackId.HasValue)
            {
                TempData["Error"] = "Invalid comment target.";
            }
            else if (postId.HasValue && feedbackId.HasValue)
            {
                TempData["Error"] = "Comment cannot belong to both Post and Feedback.";
            }
            else
            {
                var comment = new Comment
                {
                    Id = Guid.NewGuid(),
                    Content = content.Trim(),
                    PostId = postId,
                    FeedbackId = feedbackId
                };

                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Comment added successfully.";
            }

            // Redirect back to the parent entity
            if (postId.HasValue)
            {
                return RedirectToAction("Details", "Posts", new { id = postId });
            }
            else if (feedbackId.HasValue)
            {
                return RedirectToAction("Details", "Feedbacks", new { id = feedbackId });
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CommentExists(Guid id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }

        private async Task PopulateDropdowns(Guid? selectedPostId = null, Guid? selectedFeedbackId = null)
        {
            // Posts dropdown
            var posts = await _context.Posts
                .OrderBy(p => p.Title)
                .Select(p => new { p.Id, p.Title })
                .ToListAsync();

            ViewData["PostId"] = new SelectList(posts, "Id", "Title", selectedPostId);

            // Feedbacks dropdown
            var feedbacks = await _context.Feedbacks
                .OrderBy(f => f.Title)
                .Select(f => new { f.Id, f.Title })
                .ToListAsync();

            ViewData["FeedbackId"] = new SelectList(feedbacks, "Id", "Title", selectedFeedbackId);
        }
    }
}