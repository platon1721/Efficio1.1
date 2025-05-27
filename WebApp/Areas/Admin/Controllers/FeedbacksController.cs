using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.DAL.EF;
using App.Domain;
using Task = System.Threading.Tasks.Task;

namespace WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FeedbacksController : Controller
    {
        private readonly AppDbContext _context;

        public FeedbacksController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Feedbacks
        public async Task<IActionResult> Index()
        {
            var feedbacks = await _context.Feedbacks
                .Include(f => f.Department)
                .Include(f => f.FeedbackTags!)
                .ThenInclude(ft => ft.Tag)
                .Include(f => f.Comments)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            return View(feedbacks);
        }

        // GET: Admin/Feedbacks/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedbacks
                .Include(f => f.Department)
                .Include(f => f.FeedbackTags!)
                .ThenInclude(ft => ft.Tag)
                .Include(f => f.Comments)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback);
        }

        // GET: Admin/Feedbacks/Create
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View();
        }

        // POST: Admin/Feedbacks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,DepartmentId")] Feedback feedback,
            string[]? selectedTags)
        {
            if (ModelState.IsValid)
            {
                feedback.Id = Guid.NewGuid();

                // Handle tags
                if (selectedTags != null && selectedTags.Length > 0)
                {
                    feedback.FeedbackTags = new List<FeedbackTag>();
                    foreach (var tagId in selectedTags)
                    {
                        if (Guid.TryParse(tagId, out var parsedTagId))
                        {
                            feedback.FeedbackTags.Add(new FeedbackTag
                            {
                                Id = Guid.NewGuid(),
                                FeedbackId = feedback.Id,
                                TagId = parsedTagId
                            });
                        }
                    }
                }

                _context.Add(feedback);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await PopulateDropdowns(feedback.DepartmentId, selectedTags);
            return View(feedback);
        }

        // GET: Admin/Feedbacks/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedbacks
                .Include(f => f.FeedbackTags!)
                .ThenInclude(ft => ft.Tag)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (feedback == null)
            {
                return NotFound();
            }

            var selectedTags = feedback.FeedbackTags?.Select(ft => ft.TagId.ToString()).ToArray();
            await PopulateDropdowns(feedback.DepartmentId, selectedTags);

            return View(feedback);
        }

        // POST: Admin/Feedbacks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title,Description,DepartmentId")] Feedback feedback,
            string[]? selectedTags)
        {
            if (id != feedback.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Get existing feedback with tags
                    var existingFeedback = await _context.Feedbacks
                        .Include(f => f.FeedbackTags)
                        .FirstOrDefaultAsync(f => f.Id == id);

                    if (existingFeedback == null)
                    {
                        return NotFound();
                    }

                    // Update basic properties
                    existingFeedback.Title = feedback.Title;
                    existingFeedback.Description = feedback.Description;
                    existingFeedback.DepartmentId = feedback.DepartmentId;

                    // Update tags
                    if (existingFeedback.FeedbackTags != null)
                    {
                        _context.FeedbackTags.RemoveRange(existingFeedback.FeedbackTags);
                    }

                    if (selectedTags != null && selectedTags.Length > 0)
                    {
                        existingFeedback.FeedbackTags = new List<FeedbackTag>();
                        foreach (var tagId in selectedTags)
                        {
                            if (Guid.TryParse(tagId, out var parsedTagId))
                            {
                                existingFeedback.FeedbackTags.Add(new FeedbackTag
                                {
                                    Id = Guid.NewGuid(),
                                    FeedbackId = existingFeedback.Id,
                                    TagId = parsedTagId
                                });
                            }
                        }
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FeedbackExists(feedback.Id))
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

            await PopulateDropdowns(feedback.DepartmentId, selectedTags);
            return View(feedback);
        }

        // GET: Admin/Feedbacks/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedbacks
                .Include(f => f.Department)
                .Include(f => f.FeedbackTags!)
                .ThenInclude(ft => ft.Tag)
                .Include(f => f.Comments)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback);
        }

        // POST: Admin/Feedbacks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var feedback = await _context.Feedbacks
                .Include(f => f.FeedbackTags)
                .Include(f => f.Comments)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (feedback != null)
            {
                // Check if there are comments
                if (feedback.Comments?.Any() == true)
                {
                    TempData["Error"] =
                        $"Cannot delete feedback '{feedback.Title}' because it has {feedback.Comments.Count()} comment(s). Please delete the comments first.";
                    return RedirectToAction(nameof(Index));
                }

                // Remove associated tags first
                if (feedback.FeedbackTags != null)
                {
                    _context.FeedbackTags.RemoveRange(feedback.FeedbackTags);
                }

                _context.Feedbacks.Remove(feedback);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Feedback '{feedback.Title}' has been deleted successfully.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Feedbacks/AddComment/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(Guid feedbackId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["Error"] = "Comment content cannot be empty.";
                return RedirectToAction(nameof(Details), new { id = feedbackId });
            }

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                Content = content.Trim(),
                FeedbackId = feedbackId
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Comment added successfully.";
            return RedirectToAction(nameof(Details), new { id = feedbackId });
        }

        private bool FeedbackExists(Guid id)
        {
            return _context.Feedbacks.Any(e => e.Id == id);
        }

        private async Task PopulateDropdowns(Guid? selectedDepartmentId = null, string[]? selectedTags = null)
        {
            // Department dropdown with proper display
            var departments = await _context.Departments
                .OrderBy(d => d.DepartmentName)
                .Select(d => new { d.Id, d.DepartmentName })
                .ToListAsync();

            ViewData["DepartmentId"] = new SelectList(departments, "Id", "DepartmentName", selectedDepartmentId);

            // Tags for multi-select
            var allTags = await _context.Tags
                .OrderBy(t => t.Title)
                .ToListAsync();

            ViewBag.AllTags = allTags;
            ViewBag.SelectedTags = selectedTags ?? new string[0];
        }
    }
}