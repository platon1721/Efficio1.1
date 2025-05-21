using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.BLL.Contracts;
using App.BLL.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Base.Helpers;

using Microsoft.AspNetCore.Authorization;
using WebApp.ViewModels;

namespace WebApp.Controllers;

[Authorize]
public class ContactsController : Controller
{
    private readonly IAppBLL _bll;

    public ContactsController(IAppBLL bll)
    {
        _bll = bll;
    }

    // GET: Contacts
    public async Task<IActionResult> Index()
    {
        var res = await _bll.ContactService.AllAsync(User.GetUserId());
        return View(res);
    }

    // GET: Contacts/Details/5
    public async Task<IActionResult> Details(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var entity = await _bll.ContactService.FindAsync(id.Value, User.GetUserId());

        if (entity == null)
        {
            return NotFound();
        }

        return View(entity);
    }

    // GET: Contacts/Create
    public async Task<IActionResult> Create()
    {
        var vm = new ContactCreateEditViewModel()
        {
            PersonSelectList = await GetPersonSelectListAsync(),
            ContactTypeSelectList = await GetContactTypeSelectListAsync(),
        };
        return View(vm);
    }
    
    // POST: Contacts/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ContactCreateEditViewModel vm)
    {
        if (ModelState.IsValid)
        {
            _bll.ContactService.Add(vm.Contact);
            await _bll.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        vm.ContactTypeSelectList = await GetContactTypeSelectListAsync(vm.Contact.ContactTypeId);
        vm.PersonSelectList = await GetPersonSelectListAsync(vm.Contact.PersonId);

        return View(vm);
    }

    // GET: Contacts/Edit/5
    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var contact = await _bll.ContactService.FindAsync(id.Value, User.GetUserId());
        if (contact == null)
        {
            return NotFound();
        }

        var vm = new ContactCreateEditViewModel()
        {
            ContactTypeSelectList = await GetContactTypeSelectListAsync(contact.ContactTypeId),
            PersonSelectList = await GetPersonSelectListAsync(contact.PersonId),
            Contact = contact
        };
        return View(vm);
    }

    // POST: Contacts/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, ContactCreateEditViewModel vm)
    {
        if (id != vm.Contact.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            _bll.ContactService.Update(vm.Contact);
            await _bll.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        vm.ContactTypeSelectList = await GetContactTypeSelectListAsync(vm.Contact.ContactTypeId);
        vm.PersonSelectList = await GetPersonSelectListAsync(vm.Contact.PersonId);

        return View(vm);
    }

    // GET: Contacts/Delete/5
    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var contact = await _bll.ContactService.FindAsync(id.Value, User.GetUserId());

        if (contact == null)
        {
            return NotFound();
        }

        return View(contact);
    }

    // POST: Contacts/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        await _bll.ContactService.RemoveAsync(id, User.GetUserId());
        await _bll.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    
    private async Task<SelectList> GetPersonSelectListAsync(Guid? selectedValue = null)
    {
        return new SelectList(await _bll.PersonService.AllAsync(User.GetUserId()), nameof(Person.Id),
            nameof(Person.PersonName), selectedValue);
    }
    private async Task<SelectList> GetContactTypeSelectListAsync(Guid? selectedValue = null)
    {
        return new SelectList(await _bll.ContactTypeService.AllAsync(User.GetUserId()), nameof(ContactType.Id),
            nameof(ContactType.ContactTypeName), selectedValue);
    }

}