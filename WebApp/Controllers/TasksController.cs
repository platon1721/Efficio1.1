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
    public class TasksController : Controller
    {
        private readonly IAppBLL _bll;

        public TasksController(IAppBLL bll)
        {
            _bll = bll;
        }

        // GET: Tasks - Show tasks assigned to the user
        public async Task<IActionResult> Index()
        {
            var userId = User.GetUserId();
            var person = await _bll.PersonService.FindByUserIdAsync(userId);
            
            if (person == null)
            {
                // User has no person record, redirect to create profile
                return RedirectToAction("Create", "Persons");
            }

            // Get tasks assigned to this person
            var tasks = await _bll.TaskService.GetByTaskKeeperAsync(person.Id);
            return View(tasks.OrderByDescending(t => t.Id));
        }

        // GET: Tasks/MyTasks - Alternative view for user's tasks with filtering
        public async Task<IActionResult> MyTasks(string? status)
        {
            var userId = User.GetUserId();
            var person = await _bll.PersonService.FindByUserIdAsync(userId);
            
            if (person == null)
            {
                return RedirectToAction("Create", "Persons");
            }

            var tasks = await _bll.TaskService.GetByTaskKeeperAsync(person.Id);
            
            // Filter by status if provided
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<App.BLL.DTO.TaskStatus>(status, out var taskStatus))
            {
                tasks = tasks.Where(t => t.TaskStatus == taskStatus);
            }

            ViewBag.StatusFilter = status;
            ViewBag.TaskStats = new
            {
                Total = tasks.Count(),
                Open = tasks.Count(t => t.TaskStatus == App.BLL.DTO.TaskStatus.Open),
                InProgress = tasks.Count(t => t.TaskStatus == App.BLL.DTO.TaskStatus.InProgress),
                Completed = tasks.Count(t => t.TaskStatus == App.BLL.DTO.TaskStatus.Completed),
                Overdue = tasks.Count(t => t.DeadLine < DateTime.UtcNow && t.TaskStatus != App.BLL.DTO.TaskStatus.Completed)
            };

            return View(tasks.OrderByDescending(t => t.Id));
        }

        // GET: Tasks/Details/5 - Only show tasks assigned to user
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _bll.TaskService.GetWithRelationsAsync(id.Value);
            if (task == null)
            {
                return NotFound();
            }

            // Check if task is assigned to current user
            var userId = User.GetUserId();
            var person = await _bll.PersonService.FindByUserIdAsync(userId);
            
            if (person == null || task.TaskKeeperId != person.Id)
            {
                return Forbid();
            }

            return View(task);
        }

        // GET: Tasks/Edit/5 - Allow user to update task status and completion notes
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _bll.TaskService.GetWithRelationsAsync(id.Value);
            if (task == null)
            {
                return NotFound();
            }

            // Check if task is assigned to current user
            var userId = User.GetUserId();
            var person = await _bll.PersonService.FindByUserIdAsync(userId);
            
            if (person == null || task.TaskKeeperId != person.Id)
            {
                return Forbid();
            }

            // Create task status options
            var statusOptions = Enum.GetValues<App.BLL.DTO.TaskStatus>()
                .Select(s => new SelectListItem
                {
                    Value = ((int)s).ToString(),
                    Text = s.ToString(),
                    Selected = s == task.TaskStatus
                })
                .ToList();

            ViewBag.TaskStatusOptions = statusOptions;
            return View(task);
        }

        // POST: Tasks/Edit/5 - Allow updating status, completion notes
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,TaskStatus,CompletionNotes")] App.BLL.DTO.Task taskUpdate)
        {
            if (id != taskUpdate.Id)
            {
                return NotFound();
            }

            var existingTask = await _bll.TaskService.GetWithRelationsAsync(id);
            if (existingTask == null)
            {
                return NotFound();
            }

            // Check if task is assigned to current user
            var userId = User.GetUserId();
            var person = await _bll.PersonService.FindByUserIdAsync(userId);
            
            if (person == null || existingTask.TaskKeeperId != person.Id)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                // Only allow updating status and completion notes
                existingTask.TaskStatus = taskUpdate.TaskStatus;
                existingTask.CompletionNotes = taskUpdate.CompletionNotes;
                
                // Set completion date if task is being completed
                if (taskUpdate.TaskStatus == App.BLL.DTO.TaskStatus.Completed && existingTask.CompletedAt == null)
                {
                    existingTask.CompletedAt = DateTime.UtcNow;
                }
                else if (taskUpdate.TaskStatus != App.BLL.DTO.TaskStatus.Completed)
                {
                    existingTask.CompletedAt = null;
                }

                _bll.TaskService.Update(existingTask);
                await _bll.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Reload status options on error
            var statusOptions = Enum.GetValues<App.BLL.DTO.TaskStatus>()
                .Select(s => new SelectListItem
                {
                    Value = ((int)s).ToString(),
                    Text = s.ToString(),
                    Selected = s == taskUpdate.TaskStatus
                })
                .ToList();

            ViewBag.TaskStatusOptions = statusOptions;
            return View(existingTask);
        }

        // POST: Tasks/Complete/5 - Quick action to complete a task
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(Guid id, string? completionNotes)
        {
            var task = await _bll.TaskService.GetWithRelationsAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            // Check if task is assigned to current user
            var userId = User.GetUserId();
            var person = await _bll.PersonService.FindByUserIdAsync(userId);
            
            if (person == null || task.TaskKeeperId != person.Id)
            {
                return Forbid();
            }

            task.TaskStatus = App.BLL.DTO.TaskStatus.Completed;
            task.CompletedAt = DateTime.UtcNow;
            task.CompletionNotes = completionNotes;

            _bll.TaskService.Update(task);
            await _bll.SaveChangesAsync();

            TempData["Success"] = "Task completed successfully!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Tasks/UpdateStatus/5 - Quick action to update task status
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(Guid id, App.BLL.DTO.TaskStatus newStatus)
        {
            var task = await _bll.TaskService.GetWithRelationsAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            // Check if task is assigned to current user
            var userId = User.GetUserId();
            var person = await _bll.PersonService.FindByUserIdAsync(userId);
            
            if (person == null || task.TaskKeeperId != person.Id)
            {
                return Forbid();
            }

            task.TaskStatus = newStatus;
            
            // Set/clear completion date based on status
            if (newStatus == App.BLL.DTO.TaskStatus.Completed && task.CompletedAt == null)
            {
                task.CompletedAt = DateTime.UtcNow;
            }
            else if (newStatus != App.BLL.DTO.TaskStatus.Completed)
            {
                task.CompletedAt = null;
            }

            _bll.TaskService.Update(task);
            await _bll.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}