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
using Task = System.Threading.Tasks.Task;

namespace WebApp.Controllers
{
    [Authorize]
    public class CommentsController : Controller
    {
        private readonly IAppBLL _bll;

        public CommentsController(IAppBLL bll)
        {
            _bll = bll;
        }

        // GET: Comments - Show user's own comments
        public async Task<IActionResult> Index()
        {
            var userId = User.GetUserId();
            var comments = await _bll.CommentService.AllAsync(userId);
            return View(comments.OrderByDescending(c => c.Id));
        }

        // GET: Comments/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _bll.CommentService.FindAsync(id.Value, User.GetUserId());
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // GET: Comments/Create
        public async Task<IActionResult> Create(Guid? postId, Guid? feedbackId)
        {
            var userId = User.GetUserId();
            var person = await _bll.PersonService.FindByUserIdAsync(userId);
            
            if (person == null)
            {
                return RedirectToAction("Create", "Persons");
            }

            var personWithDepts = await _bll.PersonService.GetWithDepartmentsAsync(person.Id, userId);

            // Get accessible posts and feedbacks based on user's departments
            var accessiblePosts = new List<Post>();
            var accessibleFeedbacks = new List<Feedback>();

            if (personWithDepts?.Departments?.Any() == true)
            {
                foreach (var dept in personWithDepts.Departments)
                {
                    var deptPosts = await _bll.PostService.GetByDepartmentAsync(dept.Id);
                    accessiblePosts.AddRange(deptPosts);
                    
                    var deptFeedbacks = await _bll.FeedbackService.GetByDepartmentAsync(dept.Id);
                    accessibleFeedbacks.AddRange(deptFeedbacks);
                }
            }

            // Validate access if specific post or feedback is provided
            if (postId.HasValue)
            {
                if (!accessiblePosts.Any(p => p.Id == postId.Value))
                {
                    return Forbid();
                }
            }

            if (feedbackId.HasValue)
            {
                if (!accessibleFeedbacks.Any(f => f.Id == feedbackId.Value))
                {
                    return Forbid();
                }
            }

            ViewData["PostId"] = new SelectList(accessiblePosts.Distinct(), "Id", "Title", postId);
            ViewData["FeedbackId"] = new SelectList(accessibleFeedbacks.Distinct(), "Id", "Title", feedbackId);

            var comment = new Comment
            {
                PostId = postId,
                FeedbackId = feedbackId
            };

            return View(comment);
        }

        // POST: Comments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Content,PostId,FeedbackId")] Comment comment)
        {
            // Validate that comment is for either post or feedback, but not both
            if ((comment.PostId.HasValue && comment.FeedbackId.HasValue) ||
                (!comment.PostId.HasValue && !comment.FeedbackId.HasValue))
            {
                ModelState.AddModelError("", "Comment must be for either a post or feedback, but not both.");
            }

            var userId = User.GetUserId();
            var person = await _bll.PersonService.FindByUserIdAsync(userId);
            
            if (person == null)
            {
                return RedirectToAction("Create", "Persons");
            }

            var personWithDepts = await _bll.PersonService.GetWithDepartmentsAsync(person.Id, userId);

            // Validate access to post or feedback
            if (comment.PostId.HasValue)
            {
                var post = await _bll.PostService.GetWithTagsAndDepartmentsAsync(comment.PostId.Value);
                if (post == null || !HasAccessToPost(post, personWithDepts))
                {
                    ModelState.AddModelError("PostId", "You don't have access to comment on this post.");
                }
            }

            if (comment.FeedbackId.HasValue)
            {
                var feedback = await _bll.FeedbackService.FindAsync(comment.FeedbackId.Value);
                if (feedback == null || !HasAccessToFeedback(feedback, personWithDepts))
                {
                    ModelState.AddModelError("FeedbackId", "You don't have access to comment on this feedback.");
                }
            }

            if (ModelState.IsValid)
            {
                comment.Id = Guid.NewGuid();
                _bll.CommentService.Add(comment, userId);
                await _bll.SaveChangesAsync();

                // Redirect back to the post or feedback details
                if (comment.PostId.HasValue)
                {
                    return RedirectToAction("Details", "Posts", new { id = comment.PostId });
                }
                else if (comment.FeedbackId.HasValue)
                {
                    return RedirectToAction("Details", "Feedbacks", new { id = comment.FeedbackId });
                }

                return RedirectToAction(nameof(Index));
            }

            // Reload select lists on error
            await LoadSelectLists(personWithDepts, comment.PostId, comment.FeedbackId);
            return View(comment);
        }

        // GET: Comments/Edit/5 - Only allow editing own comments
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _bll.CommentService.FindAsync(id.Value, User.GetUserId());
            if (comment == null)
            {
                return NotFound();
            }

            var userId = User.GetUserId();
            var person = await _bll.PersonService.FindByUserIdAsync(userId);
            var personWithDepts = await _bll.PersonService.GetWithDepartmentsAsync(person!.Id, userId);

            await LoadSelectLists(personWithDepts, comment.PostId, comment.FeedbackId);
            return View(comment);
        }

        // POST: Comments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Content,PostId,FeedbackId")] Comment comment)
        {
            if (id != comment.Id)
            {
                return NotFound();
            }

            var existingComment = await _bll.CommentService.FindAsync(id, User.GetUserId());
            if (existingComment == null)
            {
                return NotFound();
            }

            // Validate that comment is for either post or feedback, but not both
            if ((comment.PostId.HasValue && comment.FeedbackId.HasValue) ||
                (!comment.PostId.HasValue && !comment.FeedbackId.HasValue))
            {
                ModelState.AddModelError("", "Comment must be for either a post or feedback, but not both.");
            }

            if (ModelState.IsValid)
            {
                _bll.CommentService.Update(comment);
                await _bll.SaveChangesAsync();

                // Redirect back to the post or feedback details
                if (comment.PostId.HasValue)
                {
                    return RedirectToAction("Details", "Posts", new { id = comment.PostId });
                }
                else if (comment.FeedbackId.HasValue)
                {
                    return RedirectToAction("Details", "Feedbacks", new { id = comment.FeedbackId });
                }

                return RedirectToAction(nameof(Index));
            }

            var userId = User.GetUserId();
            var person = await _bll.PersonService.FindByUserIdAsync(userId);
            var personWithDepts = await _bll.PersonService.GetWithDepartmentsAsync(person!.Id, userId);

            await LoadSelectLists(personWithDepts, comment.PostId, comment.FeedbackId);
            return View(comment);
        }

        // GET: Comments/Delete/5 - Only allow deleting own comments
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _bll.CommentService.FindAsync(id.Value, User.GetUserId());
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var comment = await _bll.CommentService.FindAsync(id, User.GetUserId());
            if (comment != null)
            {
                var postId = comment.PostId;
                var feedbackId = comment.FeedbackId;

                await _bll.CommentService.RemoveAsync(id, User.GetUserId());
                await _bll.SaveChangesAsync();

                // Redirect back to the post or feedback details
                if (postId.HasValue)
                {
                    return RedirectToAction("Details", "Posts", new { id = postId });
                }
                else if (feedbackId.HasValue)
                {
                    return RedirectToAction("Details", "Feedbacks", new { id = feedbackId });
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool HasAccessToPost(Post post, Person? personWithDepts)
        {
            if (post.Departments?.Any() != true)
                return true; // No department restrictions

            if (personWithDepts?.Departments?.Any() != true)
                return false; // User has no departments

            return post.Departments.Any(pd => 
                personWithDepts.Departments.Any(ud => ud.Id == pd.Id));
        }

        private bool HasAccessToFeedback(Feedback feedback, Person? personWithDepts)
        {
            if (!feedback.DepartmentId.HasValue)
                return true; // No department restrictions

            if (personWithDepts?.Departments?.Any() != true)
                return false; // User has no departments

            return personWithDepts.Departments.Any(d => d.Id == feedback.DepartmentId.Value);
        }

        private async Task LoadSelectLists(Person? personWithDepts, Guid? selectedPostId, Guid? selectedFeedbackId)
        {
            var accessiblePosts = new List<Post>();
            var accessibleFeedbacks = new List<Feedback>();

            if (personWithDepts?.Departments?.Any() == true)
            {
                foreach (var dept in personWithDepts.Departments)
                {
                    var deptPosts = await _bll.PostService.GetByDepartmentAsync(dept.Id);
                    accessiblePosts.AddRange(deptPosts);
                    
                    var deptFeedbacks = await _bll.FeedbackService.GetByDepartmentAsync(dept.Id);
                    accessibleFeedbacks.AddRange(deptFeedbacks);
                }
            }

            ViewData["PostId"] = new SelectList(accessiblePosts.Distinct(), "Id", "Title", selectedPostId);
            ViewData["FeedbackId"] = new SelectList(accessibleFeedbacks.Distinct(), "Id", "Title", selectedFeedbackId);
        }
    }
}