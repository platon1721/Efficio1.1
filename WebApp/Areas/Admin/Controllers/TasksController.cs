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
    public class TasksController : Controller
    {
        private readonly AppDbContext _context;

        public TasksController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Tasks
        public async Task<IActionResult> Index(string sortOrder, string searchString, string statusFilter)
        {
            ViewData["TitleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["PrioritySortParm"] = sortOrder == "Priority" ? "priority_desc" : "Priority";
            ViewData["DeadlineSortParm"] = sortOrder == "Deadline" ? "deadline_desc" : "Deadline";
            ViewData["CurrentFilter"] = searchString;
            ViewData["StatusFilter"] = statusFilter;

            var tasks = _context.Tasks
                .Include(t => t.Department)
                .Include(t => t.TaskKeeper)
                .AsQueryable();

            // Filter by search string
            if (!String.IsNullOrEmpty(searchString))
            {
                tasks = tasks.Where(t => t.Title.Contains(searchString) || 
                                        t.Description.Contains(searchString));
            }

            // Filter by status
            if (!String.IsNullOrEmpty(statusFilter) && Enum.TryParse<App.Domain.TaskStatus>(statusFilter, out var status))
            {
                tasks = tasks.Where(t => t.TaskStatus == status);
            }

            // Sort
            switch (sortOrder)
            {
                case "title_desc":
                    tasks = tasks.OrderByDescending(t => t.Title);
                    break;
                case "Priority":
                    tasks = tasks.OrderBy(t => t.Priority);
                    break;
                case "priority_desc":
                    tasks = tasks.OrderByDescending(t => t.Priority);
                    break;
                case "Deadline":
                    tasks = tasks.OrderBy(t => t.DeadLine);
                    break;
                case "deadline_desc":
                    tasks = tasks.OrderByDescending(t => t.DeadLine);
                    break;
                default:
                    tasks = tasks.OrderBy(t => t.Title);
                    break;
            }

            // Status filter options
            ViewBag.StatusOptions = new SelectList(Enum.GetValues(typeof(App.Domain.TaskStatus))
                .Cast<App.Domain.TaskStatus>()
                .Select(e => new { Value = e.ToString(), Text = e.ToString() }), "Value", "Text");

            return View(await tasks.ToListAsync());
        }

        // GET: Admin/Tasks/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .Include(t => t.Department)
                .Include(t => t.TaskKeeper)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // GET: Admin/Tasks/Create
        public IActionResult Create()
        {
            PopulateDropdowns();
            return View();
        }

        // POST: Admin/Tasks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Priority,DeadLine,TaskKeeperId,DepartmentId,TaskStatus")] App.Domain.Task task)
        {
            // Remove fields that are auto-managed
            ModelState.Remove("CreatedBy");
            ModelState.Remove("CreatedAt");
            ModelState.Remove("ChangedBy");
            ModelState.Remove("ChangedAt");
            ModelState.Remove("CompletedAt");
            ModelState.Remove("CompletionNotes");

            if (ModelState.IsValid)
            {
                task.Id = Guid.NewGuid();
                task.TaskStatus = App.Domain.TaskStatus.Open; // Default status
                _context.Add(task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopulateDropdowns(task);
            return View(task);
        }

        // GET: Admin/Tasks/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            PopulateDropdowns(task);
            return View(task);
        }

        // POST: Admin/Tasks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title,Description,Priority,DeadLine,TaskKeeperId,DepartmentId,TaskStatus,CompletionNotes")] App.Domain.Task task)
        {
            if (id != task.Id)
            {
                return NotFound();
            }

            // Remove fields that are auto-managed
            ModelState.Remove("CreatedBy");
            ModelState.Remove("CreatedAt");
            ModelState.Remove("ChangedBy");
            ModelState.Remove("ChangedAt");

            if (ModelState.IsValid)
            {
                try
                {
                    var existingTask = await _context.Tasks.FindAsync(id);
                    if (existingTask == null)
                    {
                        return NotFound();
                    }

                    // Update only the allowed fields
                    existingTask.Title = task.Title;
                    existingTask.Description = task.Description;
                    existingTask.Priority = task.Priority;
                    existingTask.DeadLine = task.DeadLine;
                    existingTask.TaskKeeperId = task.TaskKeeperId;
                    existingTask.DepartmentId = task.DepartmentId;
                    existingTask.TaskStatus = task.TaskStatus;
                    existingTask.CompletionNotes = task.CompletionNotes;

                    // Auto-set completion date if status is completed
                    if (task.TaskStatus == App.Domain.TaskStatus.Completed && existingTask.CompletedAt == null)
                    {
                        existingTask.CompletedAt = DateTime.UtcNow;
                    }
                    else if (task.TaskStatus != App.Domain.TaskStatus.Completed)
                    {
                        existingTask.CompletedAt = null;
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskExists(task.Id))
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

            PopulateDropdowns(task);
            return View(task);
        }

        // GET: Admin/Tasks/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .Include(t => t.Department)
                .Include(t => t.TaskKeeper)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // POST: Admin/Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Quick action to mark task as completed
        [HttpPost]
        public async Task<IActionResult> MarkCompleted(Guid id, string completionNotes = "")
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task != null)
            {
                task.TaskStatus = App.Domain.TaskStatus.Completed;
                task.CompletedAt = DateTime.UtcNow;
                task.CompletionNotes = completionNotes;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool TaskExists(Guid id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }

        private void PopulateDropdowns(App.Domain.Task? task = null)
        {
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "DepartmentName", task?.DepartmentId);
            ViewData["TaskKeeperId"] = new SelectList(_context.Persons, "Id", "PersonName", task?.TaskKeeperId);
            ViewData["TaskStatus"] = new SelectList(Enum.GetValues(typeof(App.Domain.TaskStatus))
                .Cast<App.Domain.TaskStatus>()
                .Select(e => new { Value = (int)e, Text = e.ToString() }), "Value", "Text", (int?)task?.TaskStatus);
            ViewData["PriorityOptions"] = new SelectList(
                Enumerable.Range(1, 5).Select(i => new { Value = i, Text = $"Priority {i}" }), 
                "Value", "Text", task?.Priority);
        }
    }
}