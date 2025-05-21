using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.BLL.Contracts;
using Microsoft.AspNetCore.Mvc;
using App.BLL.DTO;
using Base.Helpers;

using Microsoft.AspNetCore.Authorization;
using WebApp.ViewModels;

namespace WebApp.Controllers;

[Authorize]
public class ContactTypesController : Controller
{
    private readonly IAppBLL _bll;

    public ContactTypesController(IAppBLL bll)
    {
        _bll = bll;
    }

    // GET: ContactTypes
    public async Task<IActionResult> Index()
    {
        var res = await _bll.ContactTypeService.AllAsync(User.GetUserId());
        return View(res);
    }

    // GET: ContactTypes/Details/5
    public async Task<IActionResult> Details(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var entity = await _bll.ContactTypeService.FindAsync(id.Value, User.GetUserId());


        if (entity == null)
        {
            return NotFound();
        }

        return View(entity);
    }

    // GET: ContactTypes/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: ContactTypes/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ContactTypeCreateViewModel vm)
    {
  
        if (ModelState.IsValid)
        {
            var entity = new ContactType
            {
                ContactTypeName = vm.ContactTypeName,
            };
            _bll.ContactTypeService.Add(entity, User.GetUserId());
            await _bll.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(vm);
    }

    // GET: ContactTypes/Edit/5
    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var entity = await _bll.ContactTypeService.FindAsync(id.Value, User.GetUserId());
        if (entity == null)
        {
            return NotFound();
        }

        var vm = new ContactTypeEditViewModel()
        {
            Id = entity.Id,
            ContactTypeName = entity.ContactTypeName,
        };

        return View(vm);
    }

    // POST: ContactTypes/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, ContactTypeEditViewModel vm)
    {
        if (id != vm.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var entity = await _bll.ContactTypeService.FindAsync(vm.Id, User.GetUserId());
            if (entity == null)
            {
                return NotFound();
                
            }
            // TODO: handle translations
            //entity.ContactTypeName.SetTranslation(vm.ContactTypeName);
            _bll.ContactTypeService.Update(entity);
            await _bll.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(vm);
    }

    // GET: ContactTypes/Delete/5
    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }


        var entity = await _bll.ContactTypeService.FindAsync(id.Value, User.GetUserId());
        if (entity == null)
        {
            return NotFound();
        }

        return View(entity);
    }

    // POST: ContactTypes/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        await _bll.ContactTypeService.RemoveAsync(id, User.GetUserId());
        await _bll.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
   }

}