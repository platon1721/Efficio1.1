using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.BLL.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Base.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace WebApp.ApiControllers
{
    /// <summary>
    /// Task management controller
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TasksController : ControllerBase
    {
        private readonly ILogger<TasksController> _logger;
        private readonly IAppBLL _bll;

        private readonly App.DTO.v1.Mappers.TaskMapper _mapper =
            new App.DTO.v1.Mappers.TaskMapper();

        /// <summary>
        /// Constructor
        /// </summary>
        public TasksController(IAppBLL bll, ILogger<TasksController> logger)
        {
            _bll = bll;
            _logger = logger;
        }

        /// <summary>
        /// Get all tasks with relations
        /// </summary>
        /// <returns>List of tasks</returns>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<App.DTO.v1.Task>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<App.DTO.v1.Task>>> GetTasks()
        {
            var data = await _bll.TaskService.GetAllWithRelationsAsync();
            var res = data.Select(x => _mapper.Map(x)!).OrderBy(x => x.DeadLine).ToList();
            return res;
        }

        /// <summary>
        /// Get task by id with relations
        /// </summary>
        /// <param name="id">Task ID</param>
        /// <returns>Task with relations</returns>
        [HttpGet("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(App.DTO.v1.Task), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<App.DTO.v1.Task>> GetTask(Guid id)
        {
            var task = await _bll.TaskService.GetWithRelationsAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            return _mapper.Map(task)!;
        }

        /// <summary>
        /// Update task
        /// </summary>
        /// <param name="id">Task ID</param>
        /// <param name="task">Task update data</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutTask(Guid id, App.DTO.v1.UpdateTask task)
        {
            var bllTask = _mapper.Map(task, id);
            var result = await _bll.TaskService.UpdateAsync(bllTask);
            
            if (result == null)
            {
                return NotFound();
            }
            
            await _bll.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Create new task
        /// </summary>
        /// <param name="task">Task creation data</param>
        /// <returns>Created task</returns>
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(App.DTO.v1.Task), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<App.DTO.v1.Task>> PostTask(App.DTO.v1.CreateTask task)
        {
            var bllEntity = _mapper.Map(task);
            _bll.TaskService.Add(bllEntity);
            await _bll.SaveChangesAsync();

            return CreatedAtAction("GetTask", new
            {
                id = bllEntity.Id,
                version = HttpContext.GetRequestedApiVersion()!.ToString()
            }, _mapper.Map(bllEntity)!);
        }

        /// <summary>
        /// Delete task by id
        /// </summary>
        /// <param name="id">Task ID</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            var task = await _bll.TaskService.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            _bll.TaskService.Remove(id);
            await _bll.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Complete a task
        /// </summary>
        /// <param name="id">Task ID</param>
        /// <param name="completionNotes">Optional completion notes</param>
        /// <returns>Updated task</returns>
        [HttpPost("{id}/complete")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(App.DTO.v1.Task), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<App.DTO.v1.Task>> CompleteTask(Guid id, [FromBody] string? completionNotes = null)
        {
            var result = await _bll.TaskService.CompleteTaskAsync(id, completionNotes);
            
            if (result == null)
            {
                return NotFound();
            }

            return _mapper.Map(result)!;
        }

        /// <summary>
        /// Assign task to a person
        /// </summary>
        /// <param name="id">Task ID</param>
        /// <param name="personId">Person ID to assign to</param>
        /// <returns>Updated task</returns>
        [HttpPost("{id}/assign/{personId}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(App.DTO.v1.Task), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<App.DTO.v1.Task>> AssignTask(Guid id, Guid personId)
        {
            var result = await _bll.TaskService.AssignTaskAsync(id, personId);
            
            if (result == null)
            {
                return NotFound();
            }

            return _mapper.Map(result)!;
        }

        /// <summary>
        /// Get overdue tasks
        /// </summary>
        /// <returns>List of overdue tasks</returns>
        [HttpGet("overdue")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<App.DTO.v1.Task>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<App.DTO.v1.Task>>> GetOverdueTasks()
        {
            var data = await _bll.TaskService.GetOverdueTasksAsync();
            var res = data.Select(x => _mapper.Map(x)!).OrderBy(x => x.DeadLine).ToList();
            return res;
        }

        /// <summary>
        /// Get tasks by department
        /// </summary>
        /// <param name="departmentId">Department ID</param>
        /// <returns>List of department tasks</returns>
        [HttpGet("department/{departmentId}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<App.DTO.v1.Task>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<App.DTO.v1.Task>>> GetTasksByDepartment(Guid departmentId)
        {
            var data = await _bll.TaskService.GetByDepartmentAsync(departmentId);
            var res = data.Select(x => _mapper.Map(x)!).OrderBy(x => x.DeadLine).ToList();
            return res;
        }

        /// <summary>
        /// Get tasks by assigned person
        /// </summary>
        /// <param name="personId">Person ID</param>
        /// <returns>List of assigned tasks</returns>
        [HttpGet("person/{personId}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<App.DTO.v1.Task>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<App.DTO.v1.Task>>> GetTasksByPerson(Guid personId)
        {
            var data = await _bll.TaskService.GetByTaskKeeperAsync(personId);
            var res = data.Select(x => _mapper.Map(x)!).OrderBy(x => x.DeadLine).ToList();
            return res;
        }

        /// <summary>
        /// Get task statistics
        /// </summary>
        /// <returns>Task statistics</returns>
        [HttpGet("statistics")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(TaskStatistics), StatusCodes.Status200OK)]
        public async Task<ActionResult<TaskStatistics>> GetTaskStatistics()
        {
            var stats = await _bll.TaskService.GetTaskStatisticsAsync();
            return stats;
        }

        /// <summary>
        /// Change task status
        /// </summary>
        /// <param name="id">Task ID</param>
        /// <param name="status">New status</param>
        /// <returns>Updated task</returns>
        [HttpPost("{id}/status")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(App.DTO.v1.Task), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<App.DTO.v1.Task>> ChangeTaskStatus(Guid id, [FromBody] App.DTO.v1.TaskStatus status)
        {
            var bllStatus = (App.BLL.DTO.TaskStatus)status;
            var result = await _bll.TaskService.ChangeTaskStatusAsync(id, bllStatus);
            
            if (result == null)
            {
                return NotFound();
            }

            return _mapper.Map(result)!;
        }

        /// <summary>
        /// Update task priority
        /// </summary>
        /// <param name="id">Task ID</param>
        /// <param name="priority">New priority (1-5)</param>
        /// <returns>Updated task</returns>
        [HttpPost("{id}/priority")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(App.DTO.v1.Task), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<App.DTO.v1.Task>> UpdateTaskPriority(Guid id, [FromBody] int priority)
        {
            try
            {
                var result = await _bll.TaskService.UpdateTaskPriorityAsync(id, priority);
                
                if (result == null)
                {
                    return NotFound();
                }

                return _mapper.Map(result)!;
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}