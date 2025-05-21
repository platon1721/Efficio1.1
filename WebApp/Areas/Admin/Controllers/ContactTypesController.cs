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

namespace WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class ContactTypesController : Controller
    {
        private readonly AppDbContext _context;

        public ContactTypesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/ContactTypes
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.ContactTypes.Include(c => c.User);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Admin/ContactTypes/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contactType = await _context.ContactTypes
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contactType == null)
            {
                return NotFound();
            }

            return View(contactType);
        }

        // GET: Admin/ContactTypes/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FirstName");
            return View();
        }

        // POST: Admin/ContactTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ContactTypeName,UserId,Id,CreatedBy,CreatedAt,ChangedBy,ChangedAt,SysNotes")] ContactType contactType)
        {
            if (ModelState.IsValid)
            {
                contactType.Id = Guid.NewGuid();
                _context.Add(contactType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FirstName", contactType.UserId);
            return View(contactType);
        }

        // GET: Admin/ContactTypes/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contactType = await _context.ContactTypes.FindAsync(id);
            if (contactType == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FirstName", contactType.UserId);
            return View(contactType);
        }

        // POST: Admin/ContactTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ContactTypeName,UserId,Id,CreatedBy,CreatedAt,ChangedBy,ChangedAt,SysNotes")] ContactType contactType)
        {
            if (id != contactType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contactType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactTypeExists(contactType.Id))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FirstName", contactType.UserId);
            return View(contactType);
        }

        // GET: Admin/ContactTypes/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contactType = await _context.ContactTypes
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contactType == null)
            {
                return NotFound();
            }

            return View(contactType);
        }

        // POST: Admin/ContactTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var contactType = await _context.ContactTypes.FindAsync(id);
            if (contactType != null)
            {
                _context.ContactTypes.Remove(contactType);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContactTypeExists(Guid id)
        {
            return _context.ContactTypes.Any(e => e.Id == id);
        }
    }
}
