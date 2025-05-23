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
            var appDbContext = _context.Departments.Include(d => d.Manager);
            return View(await appDbContext.ToListAsync());
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
            return View();
        }

        // POST: Admin/Departments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DepartmentName,ManagerId")] Department department)
        {
            
            ModelState.Remove("CreatedBy");
            ModelState.Remove("CreatedAt");
            ModelState.Remove("ChangedBy");
            ModelState.Remove("ChangedAt");
            
            if (ModelState.IsValid)
            {
                department.Id = Guid.NewGuid();
                _context.Add(department);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine($"ModelState Error: {modelError.ErrorMessage}");
            }
    
            ViewData["ManagerId"] = new SelectList(_context.Persons, "Id", "PersonName", department.ManagerId);
            return View(department);
        }

        // GET: Admin/Departments/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }
            ViewData["ManagerId"] = new SelectList(_context.Persons, "Id", "PersonName", department.ManagerId);
            return View(department);
        }

        // POST: Admin/Departments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,DepartmentName,ManagerId")] Department department) // EEMALDATUD automaatsed väljad
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
                    var existingDepartment = await _context.Departments.FindAsync(id);
                    if (existingDepartment == null)
                    {
                        return NotFound();
                    }
                    
                    existingDepartment.DepartmentName = department.DepartmentName;
                    existingDepartment.ManagerId = department.ManagerId;
            
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