// WebApp/ApiControllers/DepartmentsController.cs - Proper implementation
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
    /// Department management controller
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DepartmentsController : ControllerBase
    {
        private readonly ILogger<DepartmentsController> _logger;
        private readonly IAppBLL _bll;

        private readonly App.DTO.v1.Mappers.DepartmentMapper _mapper =
            new App.DTO.v1.Mappers.DepartmentMapper();

        /// <summary>
        /// Constructor
        /// </summary>
        public DepartmentsController(IAppBLL bll, ILogger<DepartmentsController> logger)
        {
            _bll = bll;
            _logger = logger;
        }

        /// <summary>
        /// Get all departments with managers
        /// </summary>
        /// <returns>List of departments</returns>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<App.DTO.v1.Department>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<App.DTO.v1.Department>>> GetDepartments()
        {
            var data = await _bll.DepartmentService.GetAllWithManagerAsync();
            var res = data.Select(x => _mapper.Map(x)!).OrderBy(x => x.DepartmentName).ToList();
            return res;
        }

        /// <summary>
        /// Get department by id with full relations
        /// </summary>
        /// <param name="id">Department ID</param>
        /// <returns>Department with relations</returns>
        [HttpGet("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(App.DTO.v1.Department), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<App.DTO.v1.Department>> GetDepartment(Guid id)
        {
            var department = await _bll.DepartmentService.GetWithAllRelationsAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            return _mapper.Map(department)!;
        }

        /// <summary>
        /// Update department
        /// </summary>
        /// <param name="id">Department ID</param>
        /// <param name="department">Department data</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutDepartment(Guid id, App.DTO.v1.Department department)
        {
            if (id != department.Id)
            {
                return BadRequest();
            }

            var result = await _bll.DepartmentService.UpdateAsync(_mapper.Map(department)!);
            
            if (result == null)
            {
                return NotFound();
            }
            
            await _bll.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Create new department
        /// </summary>
        /// <param name="department">Department creation data</param>
        /// <returns>Created department</returns>
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(App.DTO.v1.Department), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<App.DTO.v1.Department>> PostDepartment(App.DTO.v1.CreateDepartment department)
        {
            var bllEntity = new App.BLL.DTO.Department
            {
                Id = Guid.NewGuid(),
                DepartmentName = department.DepartmentName,
                ManagerId = department.ManagerId
            };
            
            _bll.DepartmentService.Add(bllEntity);
            await _bll.SaveChangesAsync();

            return CreatedAtAction("GetDepartment", new
            {
                id = bllEntity.Id,
                version = HttpContext.GetRequestedApiVersion()!.ToString()
            }, _mapper.Map(bllEntity)!);
        }

        /// <summary>
        /// Delete department by id
        /// </summary>
        /// <param name="id">Department ID</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteDepartment(Guid id)
        {
            var department = await _bll.DepartmentService.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            _bll.DepartmentService.Remove(id);
            await _bll.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Get department with persons
        /// </summary>
        /// <param name="id">Department ID</param>
        /// <returns>Department with persons</returns>
        [HttpGet("{id}/persons")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(App.DTO.v1.Department), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<App.DTO.v1.Department>> GetDepartmentWithPersons(Guid id)
        {
            var department = await _bll.DepartmentService.GetWithPersonsAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            return _mapper.Map(department)!;
        }

        /// <summary>
        /// Get department with tasks
        /// </summary>
        /// <param name="id">Department ID</param>
        /// <returns>Department with tasks</returns>
        [HttpGet("{id}/tasks")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<App.DTO.v1.Task>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<App.DTO.v1.Task>>> GetDepartmentTasks(Guid id)
        {
            var department = await _bll.DepartmentService.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            var taskMapper = new App.DTO.v1.Mappers.TaskMapper();
            var tasks = await _bll.DepartmentService.GetDepartmentTasksAsync(id);
            var res = tasks.Select(x => taskMapper.Map(x)!).OrderBy(x => x.DeadLine).ToList();
            return res;
        }

        /// <summary>
        /// Get department with feedbacks
        /// </summary>
        /// <param name="id">Department ID</param>
        /// <returns>Department feedbacks</returns>
        [HttpGet("{id}/feedbacks")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<App.DTO.v1.Feedback>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<App.DTO.v1.Feedback>>> GetDepartmentFeedbacks(Guid id)
        {
            var department = await _bll.DepartmentService.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            var feedbackMapper = new App.DTO.v1.Mappers.FeedbackMapper();
            var feedbacks = await _bll.DepartmentService.GetDepartmentFeedbacksAsync(id);
            var res = feedbacks.Select(x => feedbackMapper.Map(x)!).OrderByDescending(x => x.Id).ToList();
            return res;
        }

        /// <summary>
        /// Get department statistics
        /// </summary>
        /// <param name="id">Department ID</param>
        /// <returns>Department statistics</returns>
        [HttpGet("{id}/statistics")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(DepartmentStatistics), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DepartmentStatistics>> GetDepartmentStatistics(Guid id)
        {
            var department = await _bll.DepartmentService.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            var stats = await _bll.DepartmentService.GetDepartmentStatisticsAsync(id);
            return stats;
        }

        /// <summary>
        /// Add person to department
        /// </summary>
        /// <param name="id">Department ID</param>
        /// <param name="departmentPerson">Department person data</param>
        /// <returns>Created department person relationship</returns>
        [HttpPost("{id}/persons")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(App.DTO.v1.DepartmentPerson), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<App.DTO.v1.DepartmentPerson>> AddPersonToDepartment(Guid id, App.DTO.v1.CreateDepartmentPerson departmentPerson)
        {
            if (id != departmentPerson.DepartmentId)
            {
                departmentPerson.DepartmentId = id; // Ensure consistency
            }

            var department = await _bll.DepartmentService.FindAsync(id);
            if (department == null)
            {
                return NotFound("Department not found");
            }

            var person = await _bll.PersonService.FindAsync(departmentPerson.PersonId);
            if (person == null)
            {
                return NotFound("Person not found");
            }

            var departmentPersonMapper = new App.DTO.v1.Mappers.DepartmentPersonMapper();
            var bllEntity = departmentPersonMapper.Map(departmentPerson);
            
            _bll.DepartmentPersonService.Add(bllEntity);
            await _bll.SaveChangesAsync();

            return CreatedAtAction("GetDepartment", new { id = id }, departmentPersonMapper.Map(bllEntity)!);
        }

        /// <summary>
        /// Remove person from department
        /// </summary>
        /// <param name="id">Department ID</param>
        /// <param name="personId">Person ID</param>
        /// <returns></returns>
        [HttpDelete("{id}/persons/{personId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemovePersonFromDepartment(Guid id, Guid personId)
        {
            var departmentPersons = await _bll.DepartmentPersonService.GetAllByDepartmentAsync(id);
            var departmentPerson = departmentPersons.FirstOrDefault(dp => dp.PersonId == personId);
            
            if (departmentPerson == null)
            {
                return NotFound("Person not found in this department");
            }

            _bll.DepartmentPersonService.Remove(departmentPerson.Id);
            await _bll.SaveChangesAsync();
            return NoContent();
        }
    }
}