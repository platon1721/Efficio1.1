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
    public class PersonsController : Controller
    {
        private readonly AppDbContext _context;

        public PersonsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Persons
        public async Task<IActionResult> Index()
        {
            var persons = await _context.Persons
                .Include(p => p.User)
                .Include(p => p.DepartmentPersons!)
                .ThenInclude(dp => dp.Department)
                .Include(p => p.AssignedTasks!) // Include assigned tasks
                .ThenInclude(t => t.Department)
                .ToListAsync();
            return View(persons);
        }

        // GET: Admin/Persons/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.Persons
                .Include(p => p.User)
                .Include(p => p.DepartmentPersons!)
                .ThenInclude(dp => dp.Department)
                .Include(p => p.AssignedTasks!) // Include all assigned tasks
                .ThenInclude(t => t.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (person == null)
            {
                return NotFound();
            }

            // Add task statistics to ViewBag for easy access in view
            ViewBag.TaskStats = new
            {
                TotalTasks = person.AssignedTasks?.Count ?? 0,
                OpenTasks = person.AssignedTasks?.Count(t => t.TaskStatus == App.Domain.TaskStatus.Open) ?? 0,
                InProgressTasks = person.AssignedTasks?.Count(t => t.TaskStatus == App.Domain.TaskStatus.InProgress) ?? 0,
                CompletedTasks = person.AssignedTasks?.Count(t => t.TaskStatus == App.Domain.TaskStatus.Completed) ?? 0,
                OverdueTasks = person.AssignedTasks?.Count(t => t.DeadLine < DateTime.UtcNow && t.TaskStatus != App.Domain.TaskStatus.Completed) ?? 0,
                HighPriorityTasks = person.AssignedTasks?.Count(t => t.Priority >= 4 && t.TaskStatus != App.Domain.TaskStatus.Completed) ?? 0,
                CompletionRate = person.AssignedTasks?.Any() == true 
                    ? Math.Round((double)(person.AssignedTasks.Count(t => t.TaskStatus == App.Domain.TaskStatus.Completed) * 100) / person.AssignedTasks.Count, 1)
                    : 0
            };

            return View(person);
        }

        // GET: Admin/Persons/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email");
            ViewBag.Departments = new MultiSelectList(_context.Departments, "Id", "DepartmentName");
            return View();
        }

        // POST: Admin/Persons/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PersonName,UserId")] Person person, Guid[] selectedDepartments)
        {
            // Remove auto-managed fields from validation
            ModelState.Remove("CreatedBy");
            ModelState.Remove("CreatedAt");
            ModelState.Remove("ChangedBy");
            ModelState.Remove("ChangedAt");

            if (ModelState.IsValid)
            {
                person.Id = Guid.NewGuid();
                _context.Add(person);
                await _context.SaveChangesAsync();

                // Add selected departments to person
                if (selectedDepartments != null && selectedDepartments.Length > 0)
                {
                    foreach (var departmentId in selectedDepartments)
                    {
                        var departmentPerson = new DepartmentPerson
                        {
                            DepartmentId = departmentId,
                            PersonId = person.Id
                        };
                        _context.DepartmentPersons.Add(departmentPerson);
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", person.UserId);
            ViewBag.Departments = new MultiSelectList(_context.Departments, "Id", "DepartmentName", selectedDepartments);
            return View(person);
        }

        // GET: Admin/Persons/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.Persons
                .Include(p => p.DepartmentPersons!)
                .ThenInclude(dp => dp.Department)
                .Include(p => p.AssignedTasks!) // Include tasks for edit view
                .ThenInclude(t => t.Department)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (person == null)
            {
                return NotFound();
            }

            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", person.UserId);
            
            var selectedDepartmentIds = person.DepartmentPersons?.Select(dp => dp.DepartmentId).ToArray() ?? new Guid[0];
            ViewBag.Departments = new MultiSelectList(_context.Departments, "Id", "DepartmentName", selectedDepartmentIds);

            return View(person);
        }

        // POST: Admin/Persons/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,PersonName,UserId")] Person person, Guid[] selectedDepartments)
        {
            if (id != person.Id)
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
                    var existingPerson = await _context.Persons
                        .Include(p => p.DepartmentPersons)
                        .FirstOrDefaultAsync(p => p.Id == id);

                    if (existingPerson == null)
                    {
                        return NotFound();
                    }

                    // Update basic properties
                    existingPerson.PersonName = person.PersonName;
                    existingPerson.UserId = person.UserId;

                    // Update department-person relationships
                    // Remove existing relationships
                    _context.DepartmentPersons.RemoveRange(existingPerson.DepartmentPersons!);

                    // Add new relationships
                    if (selectedDepartments != null && selectedDepartments.Length > 0)
                    {
                        foreach (var departmentId in selectedDepartments)
                        {
                            var departmentPerson = new DepartmentPerson
                            {
                                DepartmentId = departmentId,
                                PersonId = person.Id
                            };
                            _context.DepartmentPersons.Add(departmentPerson);
                        }
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonExists(person.Id))
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

            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", person.UserId);
            ViewBag.Departments = new MultiSelectList(_context.Departments, "Id", "DepartmentName", selectedDepartments);
            return View(person);
        }

        // GET: Admin/Persons/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.Persons
                .Include(p => p.User)
                .Include(p => p.DepartmentPersons!)
                .ThenInclude(dp => dp.Department)
                .Include(p => p.AssignedTasks!) // Include tasks for delete confirmation
                .ThenInclude(t => t.Department)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        // POST: Admin/Persons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var person = await _context.Persons
                .Include(p => p.DepartmentPersons)
                .Include(p => p.AssignedTasks)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (person != null)
            {
                // Check if person has active tasks
                var activeTasks = person.AssignedTasks?.Where(t => t.TaskStatus != App.Domain.TaskStatus.Completed && 
                                                                  t.TaskStatus != App.Domain.TaskStatus.Canceled).ToList();
                
                if (activeTasks?.Any() == true)
                {
                    TempData["Error"] = $"Cannot delete person. They have {activeTasks.Count} active task(s). Please complete or reassign these tasks first.";
                    return RedirectToAction(nameof(Delete), new { id });
                }

                // Remove department-person relationships
                if (person.DepartmentPersons?.Any() == true)
                {
                    _context.DepartmentPersons.RemoveRange(person.DepartmentPersons);
                }

                // Update tasks to remove person assignment
                if (person.AssignedTasks?.Any() == true)
                {
                    foreach (var task in person.AssignedTasks)
                    {
                        task.TaskKeeperId = null;
                    }
                }

                _context.Persons.Remove(person);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PersonExists(Guid id)
        {
            return _context.Persons.Any(e => e.Id == id);
        }
    }
}