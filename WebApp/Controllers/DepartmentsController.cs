using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using App.BLL.Contracts;
using App.BLL.DTO;
using Base.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace WebApp.Controllers
{
    [Authorize]
    public class DepartmentsController : Controller
    {
        private readonly IAppBLL _bll;

        public DepartmentsController(IAppBLL bll)
        {
            _bll = bll;
        }

        // GET: Departments - Show departments user belongs to
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
                // User not in any department, show message
                ViewBag.Message = "You are not assigned to any department. Please contact your administrator.";
                return View(new List<App.BLL.DTO.Department>());
            }

            // Get full department details with statistics
            var departments = new List<App.BLL.DTO.Department>();
            foreach (var dept in personWithDepts.Departments)
            {
                var fullDept = await _bll.DepartmentService.GetWithAllRelationsAsync(dept.Id);
                if (fullDept != null)
                {
                    departments.Add(fullDept);
                }
            }

            return View(departments.OrderBy(d => d.DepartmentName));
        }

        // GET: Departments/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Check if user has access to this department
            var userId = User.GetUserId();
            var person = await _bll.PersonService.FindByUserIdAsync(userId);
            if (person == null)
            {
                return Forbid();
            }

            var personWithDepts = await _bll.PersonService.GetWithDepartmentsAsync(person.Id, userId);
            if (personWithDepts?.Departments?.Any(d => d.Id == id.Value) != true)
            {
                return Forbid();
            }

            var department = await _bll.DepartmentService.GetWithAllRelationsAsync(id.Value);
            if (department == null)
            {
                return NotFound();
            }

            // Calculate statistics for ViewBag
            ViewBag.Stats = new
            {
                TotalMembers = department.Persons?.Count() ?? 0,
                TotalTasks = department.Tasks?.Count() ?? 0,
                OpenTasks = department.Tasks?.Count(t => t.TaskStatus == App.BLL.DTO.TaskStatus.Open) ?? 0,
                InProgressTasks = department.Tasks?.Count(t => t.TaskStatus == App.BLL.DTO.TaskStatus.InProgress) ?? 0,
                CompletedTasks = department.Tasks?.Count(t => t.TaskStatus == App.BLL.DTO.TaskStatus.Completed) ?? 0,
                OverdueTasks = department.Tasks?.Count(t => 
                    t.DeadLine < DateTime.UtcNow && t.TaskStatus != App.BLL.DTO.TaskStatus.Completed) ?? 0,
                TotalFeedbacks = department.Feedbacks?.Count() ?? 0,
                RecentFeedbacks = department.Feedbacks?.Count(f => f.Id != Guid.Empty) ?? 0 // Placeholder for created date check
            };

            return View(department);
        }

        // Non-admin users cannot create, edit, or delete departments
        // These actions are admin-only in the Admin area
    }
}