using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using App.BLL.Contracts;
using App.DAL.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.DAL.EF;
using App.DAL.EF.Repositories;
using App.BLL.DTO;
using Base.Helpers;

using Microsoft.AspNetCore.Authorization;
using WebApp.ViewModels;


namespace WebApp.Controllers;

[Authorize]
public class PersonsController : Controller
{ 
    private readonly IAppBLL _bll;

    
    public PersonsController(IAppBLL bll)
    {
        _bll = bll;
    }

    // GET: Persons
    public async Task<IActionResult> Index()
    {
        
        var res = new PersonIndexViewModel()
        {
            Persons = (await _bll.PersonService.AllAsync(User.GetUserId())).ToList(),
            // PersonCountByName = await _bll.PersonService.GetPersonCountByNameAsync("Mikk", User.GetUserId())
        };
        return View(res);
    }


    // GET: Persons/Details/5
    public async Task<IActionResult> Details(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var entity = await _bll.PersonService.FindAsync(id.Value, User.GetUserId());


        if (entity == null)
        {
            return NotFound();
        }

        return View(entity);
    }

    // GET: Persons/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Persons/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Person entity)
    {
        
        if (ModelState.IsValid)
        {
            _bll.PersonService.Add(entity, User.GetUserId());
            await _bll.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(entity);
    }

    // GET: Persons/Edit/5
    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }


        var entity = await _bll.PersonService.FindAsync(id.Value, User.GetUserId());
        if (entity == null)
        {
            return NotFound();
        }

        return View(entity);
    }

    // POST: Persons/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, Person entity)
    {
        if (id != entity.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            _bll.PersonService.Update(entity);
            await _bll.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(entity);
    }

    // GET: Persons/Delete/5
    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }


        var entity = await _bll.PersonService.FindAsync(id.Value, User.GetUserId());
        if (entity == null)
        {
            return NotFound();
        }

        return View(entity);
    }

    // POST: Persons/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        await _bll.PersonService.RemoveAsync(id, User.GetUserId());

        await _bll.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}