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
                .FirstOrDefaultAsync(m => m.Id == id);
            if (department == null)
            {
                return NotFound();
            }

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
        public async Task<IActionResult> Create([Bind("DepartmentName,ManagerId")] Department department, Guid[] selectedPersons)
        {
            ModelState.Remove("CreatedBy");
            ModelState.Remove("CreatedAt");
            ModelState.Remove("ChangedBy");
            ModelState.Remove("ChangedAt");
            
            if (ModelState.IsValid)
            {
                department.Id = Guid.NewGuid();
                _context.Add(department);
                
                // Add selected persons to department
                if (selectedPersons != null && selectedPersons.Length > 0)
                {
                    foreach (var personId in selectedPersons)
                    {
                        var departmentPerson = new DepartmentPerson
                        {
                            Id = Guid.NewGuid(),
                            DepartmentId = department.Id,
                            PersonId = personId
                        };
                        _context.DepartmentPersons.Add(departmentPerson);
                    }
                }
                
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine($"ModelState Error: {modelError.ErrorMessage}");
            }
    
            ViewBag.ManagerId = new SelectList(_context.Persons, "Id", "PersonName", department.ManagerId);
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
                .FirstOrDefaultAsync(d => d.Id == id);
            
            if (department == null)
            {
                return NotFound();
            }
            
            var selectedPersonIds = department.DepartmentPersons?.Select(dp => dp.PersonId).ToArray() ?? new Guid[0];
            
            ViewBag.ManagerId = new SelectList(_context.Persons, "Id", "PersonName", department.ManagerId);
            ViewBag.Persons = new MultiSelectList(_context.Persons, "Id", "PersonName", selectedPersonIds);
            return View(department);
        }

        // POST: Admin/Departments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,DepartmentName,ManagerId")] Department department, Guid[] selectedPersons)
        {
            if (id != department.Id)
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
                    var existingDepartment = await _context.Departments
                        .Include(d => d.DepartmentPersons!)
                        .FirstOrDefaultAsync(d => d.Id == id);
                    
                    if (existingDepartment == null)
                    {
                        return NotFound();
                    }
                    
                    existingDepartment.DepartmentName = department.DepartmentName;
                    existingDepartment.ManagerId = department.ManagerId;
                    
                    // ADDED: Explicitly mark ManagerId as modified to ensure EF tracks the change
                    _context.Entry(existingDepartment).Property(d => d.ManagerId).IsModified = true;
                    
                    // Update department persons - remove existing and add new ones
                    if (existingDepartment.DepartmentPersons != null)
                    {
                        _context.DepartmentPersons.RemoveRange(existingDepartment.DepartmentPersons);
                    }
                    
                    if (selectedPersons != null && selectedPersons.Length > 0)
                    {
                        foreach (var personId in selectedPersons)
                        {
                            var departmentPerson = new DepartmentPerson
                            {
                                Id = Guid.NewGuid(),
                                DepartmentId = existingDepartment.Id,
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
    
            ViewBag.ManagerId = new SelectList(_context.Persons, "Id", "PersonName", department.ManagerId);
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
            var department = await _context.Departments.FindAsync(id);
            if (department != null)
            {
                _context.Departments.Remove(department);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DepartmentExists(Guid id)
        {
            return _context.Departments.Any(e => e.Id == id);
        }
    }
}