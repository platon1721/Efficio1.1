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
    public class PostsController : Controller
    {
        private readonly IAppBLL _bll;

        public PostsController(IAppBLL bll)
        {
            _bll = bll;
        }

        // GET: Posts - Show posts user has access to
        public async Task<IActionResult> Index()
        {
            var userId = User.GetUserId();
            var person = await _bll.PersonService.FindByUserIdAsync(userId);
            
            if (person == null)
            {
                // User has no person record, redirect to create profile
                return RedirectToAction("Create", "Persons");
            }

            // Get all posts with full details
            var allPosts = await _bll.PostService.GetAllWithTagsDepartmentsAndCommentsAsync();
            
            // Get person with departments
            var personWithDepts = await _bll.PersonService.GetWithDepartmentsAsync(person.Id, userId);
            var userDepartmentIds = personWithDepts?.Departments?.Select(d => d.Id).ToList() ?? new List<Guid>();

            // Filter posts: show posts with no departments OR posts assigned to user's departments
            var accessiblePosts = allPosts.Where(post =>
                // Post has no department restrictions (available to everyone)
                !post.Departments?.Any() == true ||
                // OR post is assigned to at least one of user's departments
                (post.Departments?.Any() == true && userDepartmentIds.Any() && 
                 post.Departments.Any(pd => userDepartmentIds.Contains(pd.Id)))
            ).OrderByDescending(p => p.Id);

            // Add user department info to ViewBag for display purposes
            ViewBag.UserDepartments = personWithDepts?.Departments?.ToList() ?? new List<App.BLL.DTO.Department>();
            ViewBag.HasDepartments = userDepartmentIds.Any();

            return View(accessiblePosts);
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _bll.PostService.GetWithTagsDepartmentsAndCommentsAsync(id.Value);
            if (post == null)
            {
                return NotFound();
            }

            // Check if user has access to this post
            var userId = User.GetUserId();
            var person = await _bll.PersonService.FindByUserIdAsync(userId);
            if (person == null)
            {
                return Forbid();
            }

            var personWithDepts = await _bll.PersonService.GetWithDepartmentsAsync(person.Id, userId);
            var userDepartmentIds = personWithDepts?.Departments?.Select(d => d.Id).ToList() ?? new List<Guid>();

            // Check access: post has no departments OR user is in at least one post department
            var hasAccess = !post.Departments?.Any() == true || // No department restrictions
                           (post.Departments?.Any() == true && userDepartmentIds.Any() && 
                            post.Departments.Any(pd => userDepartmentIds.Contains(pd.Id))); // User in post department

            if (!hasAccess)
            {
                return Forbid();
            }

            return View(post);
        }

        // Non-admin users cannot create, edit, or delete posts
        // These actions are admin-only in the Admin area
    }
}