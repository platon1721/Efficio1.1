using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using App.BLL.Contracts;
using App.BLL.DTO;
using Base.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace WebApp.Controllers
{
    [Authorize]
    public class FeedbacksController : Controller
    {
        private readonly IAppBLL _bll;

        public FeedbacksController(IAppBLL bll)
        {
            _bll = bll;
        }

        // GET: Feedbacks - Show all feedbacks for departments user belongs to
        public async Task<IActionResult> Index()
        {
            var userId = User.GetUserId();
            var person = await _bll.PersonService.FindByUserIdAsync(userId);
            
            if (person == null)
            {
                // User has no person record, redirect to create profile
                return RedirectToAction("Create", "Persons");
            }

            // Get person with departments
            var personWithDepts = await _bll.PersonService.GetWithDepartmentsAsync(person.Id, userId);
            
            if (personWithDepts?.Departments == null || !personWithDepts.Departments.Any())
            {
                // User not in any department, show empty view with message
                ViewBag.Message = "You are not assigned to any department. Please contact your administrator.";
                return View(new List<Feedback>());
            }

            // Get feedbacks from all user's departments
            var feedbacks = new List<Feedback>();
            foreach (var dept in personWithDepts.Departments)
            {
                var deptFeedbacks = await _bll.FeedbackService.GetByDepartmentAsync(dept.Id);
                feedbacks.AddRange(deptFeedbacks);
            }

            return View(feedbacks.Distinct().OrderByDescending(f => f.Id));
        }

        // GET: Feedbacks/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _bll.FeedbackService.GetWithTagsAndCommentsAsync(id.Value);
            if (feedback == null)
            {
                return NotFound();
            }

            // Check if user has access to this feedback (is in the department)
            var userId = User.GetUserId();
            var person = await _bll.PersonService.FindByUserIdAsync(userId);
            if (person == null)
            {
                return Forbid();
            }

            var personWithDepts = await _bll.PersonService.GetWithDepartmentsAsync(person.Id, userId);
            if (feedback.DepartmentId.HasValue && 
                personWithDepts?.Departments?.Any(d => d.Id == feedback.DepartmentId.Value) != true)
            {
                return Forbid();
            }

            return View(feedback);
        }

        // GET: Feedbacks/Create
        public async Task<IActionResult> Create(Guid? departmentId)
        {
            var userId = User.GetUserId();
            var person = await _bll.PersonService.FindByUserIdAsync(userId);
            
            if (person == null)
            {
                return RedirectToAction("Create", "Persons");
            }

            var personWithDepts = await _bll.PersonService.GetWithDepartmentsAsync(person.Id, userId);
            
            if (personWithDepts?.Departments == null || !personWithDepts.Departments.Any())
            {
                TempData["Error"] = "You must be assigned to a department to create feedback.";
                return RedirectToAction("Index");
            }

            // Create department select list from user's departments
            var departments = personWithDepts.Departments.ToList();
            ViewData["DepartmentId"] = new SelectList(departments, "Id", "DepartmentName", departmentId);

            var feedback = new Feedback();
            if (departmentId.HasValue && departments.Any(d => d.Id == departmentId.Value))
            {
                feedback.DepartmentId = departmentId.Value;
            }

            return View(feedback);
        }

        // POST: Feedbacks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,DepartmentId")] Feedback feedback)
        {
            var userId = User.GetUserId();
            var person = await _bll.PersonService.FindByUserIdAsync(userId);
            
            if (person == null)
            {
                return RedirectToAction("Create", "Persons");
            }

            var personWithDepts = await _bll.PersonService.GetWithDepartmentsAsync(person.Id, userId);
            
            // Validate user can create feedback for selected department
            if (feedback.DepartmentId.HasValue)
            {
                if (personWithDepts?.Departments?.Any(d => d.Id == feedback.DepartmentId.Value) != true)
                {
                    ModelState.AddModelError("DepartmentId", "You can only create feedback for departments you belong to.");
                }
            }

            if (ModelState.IsValid)
            {
                feedback.Id = Guid.NewGuid();
                _bll.FeedbackService.Add(feedback, userId);
                await _bll.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Reload department list
            var departments = personWithDepts?.Departments?.ToList() ?? new List<Department>();
            ViewData["DepartmentId"] = new SelectList(departments, "Id", "DepartmentName", feedback.DepartmentId);
            return View(feedback);
        }

        // GET: Feedbacks/Edit/5 - Only allow editing own feedback
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _bll.FeedbackService.FindAsync(id.Value, User.GetUserId());
            if (feedback == null)
            {
                return NotFound();
            }

            var userId = User.GetUserId();
            var person = await _bll.PersonService.FindByUserIdAsync(userId);
            if (person == null)
            {
                return Forbid();
            }

            var personWithDepts = await _bll.PersonService.GetWithDepartmentsAsync(person.Id, userId);
            var departments = personWithDepts?.Departments?.ToList() ?? new List<Department>();
            
            ViewData["DepartmentId"] = new SelectList(departments, "Id", "DepartmentName", feedback.DepartmentId);
            return View(feedback);
        }

        // POST: Feedbacks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title,Description,DepartmentId")] Feedback feedback)
        {
            if (id != feedback.Id)
            {
                return NotFound();
            }

            var userId = User.GetUserId();
            var existingFeedback = await _bll.FeedbackService.FindAsync(id, userId);
            if (existingFeedback == null)
            {
                return NotFound();
            }

            var person = await _bll.PersonService.FindByUserIdAsync(userId);
            var personWithDepts = await _bll.PersonService.GetWithDepartmentsAsync(person!.Id, userId);
            
            // Validate user can assign feedback to selected department
            if (feedback.DepartmentId.HasValue)
            {
                if (personWithDepts?.Departments?.Any(d => d.Id == feedback.DepartmentId.Value) != true)
                {
                    ModelState.AddModelError("DepartmentId", "You can only assign feedback to departments you belong to.");
                }
            }

            if (ModelState.IsValid)
            {
                _bll.FeedbackService.Update(feedback);
                await _bll.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var departments = personWithDepts?.Departments?.ToList() ?? new List<Department>();
            ViewData["DepartmentId"] = new SelectList(departments, "Id", "DepartmentName", feedback.DepartmentId);
            return View(feedback);
        }

        // GET: Feedbacks/Delete/5 - Only allow deleting own feedback
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _bll.FeedbackService.FindAsync(id.Value, User.GetUserId());
            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback);
        }

        // POST: Feedbacks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var feedback = await _bll.FeedbackService.FindAsync(id, User.GetUserId());
            if (feedback != null)
            {
                await _bll.FeedbackService.RemoveAsync(id, User.GetUserId());
                await _bll.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}