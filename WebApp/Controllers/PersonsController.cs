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
        var userId = User.GetUserId();
        var persons = (await _bll.PersonService.AllAsync(userId)).ToList();
        
        var res = new PersonIndexViewModel()
        {
            Persons = persons,
            PersonCountByName = 0 // You can implement this if needed: await _bll.PersonService.GetPersonCountByNameAsync("Mikk", userId)
        };
        
        // Add some helpful information to ViewBag
        ViewBag.HasPersons = persons.Any();
        ViewBag.TotalPersons = persons.Count;
        
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
        var userId = User.GetUserId();
        
        // Check if user already has a person record
        var existingPersons = _bll.PersonService.AllAsync(userId).Result;
        if (existingPersons.Any())
        {
            TempData["Info"] = "You already have a profile. You can edit your existing profile instead.";
            return RedirectToAction(nameof(Index));
        }
        
        return View();
    }

    // POST: Persons/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Person entity)
    {
        var userId = User.GetUserId();
        
        // Check if user already has a person record
        var existingPersons = await _bll.PersonService.AllAsync(userId);
        if (existingPersons.Any())
        {
            TempData["Error"] = "You already have a profile. You can only have one profile per account.";
            return RedirectToAction(nameof(Index));
        }
        
        if (ModelState.IsValid)
        {
            try
            {
                _bll.PersonService.Add(entity, userId);
                await _bll.SaveChangesAsync();
                
                TempData["Success"] = "Your profile has been created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while creating your profile. Please try again.";
                // Log the exception if you have logging configured
            }
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

        // Store original name for comparison
        ViewBag.OriginalName = entity.PersonName;
        
        return View(entity);
    }

    // POST: Persons/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, Person entity)
    {
        if (id != entity.Id)
        {
            return NotFound();
        }

        // Verify that the person belongs to the current user
        var existingEntity = await _bll.PersonService.FindAsync(id, User.GetUserId());
        if (existingEntity == null)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                // Update only the fields that should be editable by users
                existingEntity.PersonName = entity.PersonName;
                
                _bll.PersonService.Update(existingEntity);
                await _bll.SaveChangesAsync();
                
                TempData["Success"] = "Your profile has been updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while updating your profile. Please try again.";
                // Log the exception if you have logging configured
            }
        }

        // Store original name for comparison on error
        ViewBag.OriginalName = existingEntity?.PersonName;
        
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

        // Add warning about deletion consequences
        ViewBag.DeletionWarning = "Deleting your profile may affect your ability to access tasks and department features.";

        return View(entity);
    }

    // POST: Persons/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        try
        {
            var entity = await _bll.PersonService.FindAsync(id, User.GetUserId());
            if (entity == null)
            {
                TempData["Error"] = "Profile not found or you don't have permission to delete it.";
                return RedirectToAction(nameof(Index));
            }

            await _bll.PersonService.RemoveAsync(id, User.GetUserId());
            await _bll.SaveChangesAsync();
            
            TempData["Success"] = "Your profile has been deleted successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "An error occurred while deleting your profile. Please try again.";
            // Log the exception if you have logging configured
        }

        return RedirectToAction(nameof(Index));
    }

    // Helper action to get current user's person record
    private async Task<Person?> GetCurrentUserPersonAsync()
    {
        var userId = User.GetUserId();
        var persons = await _bll.PersonService.AllAsync(userId);
        return persons.FirstOrDefault();
    }

    // GET: Check if user has profile (for AJAX calls)
    [HttpGet]
    public async Task<IActionResult> CheckProfile()
    {
        var person = await GetCurrentUserPersonAsync();
        return Json(new { hasProfile = person != null, personId = person?.Id });
    }

    // POST: Quick profile creation (simplified version)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> QuickCreate(string personName)
    {
        if (string.IsNullOrWhiteSpace(personName))
        {
            TempData["Error"] = "Please provide a valid name for your profile.";
            return RedirectToAction(nameof(Index));
        }

        var userId = User.GetUserId();
        
        // Check if user already has a person record
        var existingPersons = await _bll.PersonService.AllAsync(userId);
        if (existingPersons.Any())
        {
            TempData["Error"] = "You already have a profile.";
            return RedirectToAction(nameof(Index));
        }

        var entity = new Person
        {
            PersonName = personName.Trim()
        };

        try
        {
            _bll.PersonService.Add(entity, userId);
            await _bll.SaveChangesAsync();
            
            TempData["Success"] = $"Profile '{personName}' created successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = "An error occurred while creating your profile. Please try again.";
            return RedirectToAction(nameof(Index));
        }
    }
}