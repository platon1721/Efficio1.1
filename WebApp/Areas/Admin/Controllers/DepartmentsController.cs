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
    public class DepartmentsController : Controller
    {
        private readonly AppDbContext _context;

        public DepartmentsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Departments
        public async Task<IActionResult> Index()
        {
            var departments = await _context.Departments
                .Include(d => d.Manager)
                .Include(d => d.DepartmentPersons!)
                .ThenInclude(dp => dp.Person)
                .Include(d => d.Tasks!)
                .ThenInclude(t => t.TaskKeeper)
                .Include(d => d.Feedbacks!)
                .ToListAsync();
            return View(departments);
        }

        // GET: Admin/Departments/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .Include(d => d.Manager)
                .Include(d => d.DepartmentPersons!)
                .ThenInclude(dp => dp.Person)
                .Include(d => d.PostDepartments!)
                .ThenInclude(pd => pd.Post)
                .Include(d => d.Tasks!) // Include tasks with full details
                .ThenInclude(t => t.TaskKeeper)
                .Include(d => d.Feedbacks!) // Include feedbacks with tags and comments
                .ThenInclude(f => f.FeedbackTags!)
                .ThenInclude(ft => ft.Tag)
                .Include(d => d.Feedbacks!)
                .ThenInclude(f => f.Comments)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (department == null)
            {
                return NotFound();
            }

            // Add task statistics to ViewBag for easy access in view
            ViewBag.TaskStats = new
            {
                TotalTasks = department.Tasks?.Count ?? 0,
                OpenTasks = department.Tasks?.Count(t => t.TaskStatus == App.Domain.TaskStatus.Open) ?? 0,
                InProgressTasks = department.Tasks?.Count(t => t.TaskStatus == App.Domain.TaskStatus.InProgress) ?? 0,
                CompletedTasks = department.Tasks?.Count(t => t.TaskStatus == App.Domain.TaskStatus.Completed) ?? 0,
                OverdueTasks = department.Tasks?.Count(t =>
                    t.DeadLine < DateTime.UtcNow && t.TaskStatus != App.Domain.TaskStatus.Completed) ?? 0,
                HighPriorityTasks =
                    department.Tasks?.Count(t => t.Priority >= 4 && t.TaskStatus != App.Domain.TaskStatus.Completed) ??
                    0
            };

            ViewBag.FeedbackStats = new
            {
                TotalFeedbacks = department.Feedbacks?.Count ?? 0,
                RecentFeedbacks = department.Feedbacks?.Count(f => f.CreatedAt >= DateTime.UtcNow.AddDays(-30)) ?? 0,
                FeedbacksWithComments = department.Feedbacks?.Count(f => f.Comments != null && f.Comments.Any()) ?? 0
            };

            // Add ViewBag data for the task partial view
            ViewBag.EntityType = "Department";
            ViewBag.EntityName = department.DepartmentName;
            ViewBag.EntityId = department.Id;

            return View(department);
        }

        // GET: Admin/Departments/Create
        public IActionResult Create()
        {
            ViewData["ManagerId"] = new SelectList(_context.Persons, "Id", "PersonName");
            ViewBag.Persons = new MultiSelectList(_context.Persons, "Id", "PersonName");
            return View();
        }

        // POST: Admin/Departments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DepartmentName,ManagerId")] Department department,
            Guid[] selectedPersons)
        {
            // Remove auto-managed fields from validation
            ModelState.Remove("CreatedBy");
            ModelState.Remove("CreatedAt");
            ModelState.Remove("ChangedBy");
            ModelState.Remove("ChangedAt");

            if (ModelState.IsValid)
            {
                department.Id = Guid.NewGuid();
                _context.Add(department);
                await _context.SaveChangesAsync();

                // Add selected persons to department
                if (selectedPersons != null && selectedPersons.Length > 0)
                {
                    foreach (var personId in selectedPersons)
                    {
                        var departmentPerson = new DepartmentPerson
                        {
                            DepartmentId = department.Id,
                            PersonId = personId
                        };
                        _context.DepartmentPersons.Add(departmentPerson);
                    }

                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["ManagerId"] = new SelectList(_context.Persons, "Id", "PersonName", department.ManagerId);
            ViewBag.Persons = new MultiSelectList(_context.Persons, "Id", "PersonName", selectedPersons);
            return View(department);
        }

        // GET: Admin/Departments/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .Include(d => d.DepartmentPersons!)
                .ThenInclude(dp => dp.Person)
                .Include(d => d.Tasks!) // Include tasks for edit view
                .ThenInclude(t => t.TaskKeeper)
                .Include(d => d.Feedbacks!) // Include feedbacks for edit view
                .FirstOrDefaultAsync(d => d.Id == id);

            if (department == null)
            {
                return NotFound();
            }

            ViewData["ManagerId"] = new SelectList(_context.Persons, "Id", "PersonName", department.ManagerId);

            var selectedPersonIds = department.DepartmentPersons?.Select(dp => dp.PersonId).ToArray() ?? new Guid[0];
            ViewBag.Persons = new MultiSelectList(_context.Persons, "Id", "PersonName", selectedPersonIds);

            return View(department);
        }

        // POST: Admin/Departments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,DepartmentName,ManagerId")] Department department,
            Guid[] selectedPersons)
        {
            if (id != department.Id)
            {
                return NotFound();
            }

            // Remove auto-managed fields from validation
            ModelState.Remove("CreatedBy");
            ModelState.Remove("CreatedAt");
            ModelState.Remove("ChangedBy");
            ModelState.Remove("ChangedAt");

            if (ModelState.IsValid)
            {
                try
                {
                    var existingDepartment = await _context.Departments
                        .Include(d => d.DepartmentPersons)
                        .FirstOrDefaultAsync(d => d.Id == id);

                    if (existingDepartment == null)
                    {
                        return NotFound();
                    }

                    // Update basic properties
                    existingDepartment.DepartmentName = department.DepartmentName;
                    existingDepartment.ManagerId = department.ManagerId;

                    // Update department-person relationships
                    // Remove existing relationships
                    _context.DepartmentPersons.RemoveRange(existingDepartment.DepartmentPersons!);

                    // Add new relationships
                    if (selectedPersons != null && selectedPersons.Length > 0)
                    {
                        foreach (var personId in selectedPersons)
                        {
                            var departmentPerson = new DepartmentPerson
                            {
                                DepartmentId = department.Id,
                                PersonId = personId
                            };
                            _context.DepartmentPersons.Add(departmentPerson);
                        }
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartmentExists(department.Id))
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

            ViewData["ManagerId"] = new SelectList(_context.Persons, "Id", "PersonName", department.ManagerId);
            ViewBag.Persons = new MultiSelectList(_context.Persons, "Id", "PersonName", selectedPersons);
            return View(department);
        }

        // GET: Admin/Departments/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .Include(d => d.Manager)
                .Include(d => d.DepartmentPersons!)
                .ThenInclude(dp => dp.Person)
                .Include(d => d.Tasks!) // Include tasks for delete confirmation
                .ThenInclude(t => t.TaskKeeper)
                .Include(d => d.Feedbacks!) // Include feedbacks for delete confirmation
                .ThenInclude(f => f.Comments)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // POST: Admin/Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var department = await _context.Departments
                .Include(d => d.DepartmentPersons)
                .Include(d => d.Tasks)
                .Include(d => d.Feedbacks)
                .ThenInclude(f => f.Comments)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (department != null)
            {
                // Check if department has active tasks
                var activeTasks = department.Tasks?.Where(t => t.TaskStatus != App.Domain.TaskStatus.Completed &&
                                                               t.TaskStatus != App.Domain.TaskStatus.Canceled).ToList();

                if (activeTasks?.Any() == true)
                {
                    TempData["Error"] =
                        $"Cannot delete department. It has {activeTasks.Count} active task(s). Please complete or reassign these tasks first.";
                    return RedirectToAction(nameof(Delete), new { id });
                }

                // Check if department has feedbacks with comments
                var feedbacksWithComments =
                    department.Feedbacks?.Where(f => f.Comments != null && f.Comments.Any()).ToList();
                if (feedbacksWithComments?.Any() == true)
                {
                    TempData["Error"] =
                        $"Cannot delete department. It has {feedbacksWithComments.Count} feedback(s) with comments. Please handle the feedbacks first.";
                    return RedirectToAction(nameof(Delete), new { id });
                }

                // Remove department-person relationships
                if (department.DepartmentPersons?.Any() == true)
                {
                    _context.DepartmentPersons.RemoveRange(department.DepartmentPersons);
                }

                // Update tasks to remove department assignment
                if (department.Tasks?.Any() == true)
                {
                    foreach (var task in department.Tasks)
                    {
                        task.DepartmentId = null;
                    }
                }

                // Update feedbacks to remove department assignment
                if (department.Feedbacks?.Any() == true)
                {
                    foreach (var feedback in department.Feedbacks)
                    {
                        feedback.DepartmentId = null;
                    }
                }

                _context.Departments.Remove(department);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool DepartmentExists(Guid id)
        {
            return _context.Departments.Any(e => e.Id == id);
        }
    }
}