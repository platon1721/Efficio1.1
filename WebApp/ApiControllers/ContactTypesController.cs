using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.BLL.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using App.DAL.EF;
using Asp.Versioning;
using Base.Contracts;
using Base.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;


namespace WebApp.ApiControllers
{
    /// <inheritdoc />
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class ContactTypesController : ControllerBase
    {
        private readonly ILogger<ContactTypesController> _logger;
        private readonly IAppBLL _bll;

        private readonly App.DTO.v1.Mappers.ContactTypeMapper _mapper =
            new App.DTO.v1.Mappers.ContactTypeMapper();

        /// <inheritdoc />
        public ContactTypesController(IAppBLL bll, ILogger<ContactTypesController> logger)
        {
            _bll = bll;
            _logger = logger;
        }

        /// <summary>
        /// Get all contact types for current user
        /// </summary>
        /// <returns></returns>
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<App.DTO.v1.ContactType>), StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<App.DTO.v1.ContactType>>> GetContactTypes()
        {
            var data = await _bll.ContactTypeService.AllAsync(User.GetUserId());
            var res = data.Select(x => _mapper.Map(x)!).ToList();
            return res;
        }

        /// <summary>
        /// Get single contact type by id - owned by current user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Produces("application/json")]
        [ProducesResponseType(typeof(App.DTO.v1.ContactType), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<ActionResult<App.DTO.v1.ContactType>> GetContactType(Guid id)
        {
            var contactType = await _bll.ContactTypeService.FindAsync(id, User.GetUserId());

            if (contactType == null)
            {
                return NotFound();
            }

            return _mapper.Map(contactType)!;
        }

        /// <summary>
        /// Update contact type by id - owned by current user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="contactType"></param>
        /// <returns></returns>
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContactType(Guid id, App.DTO.v1.ContactType contactType)
        {
            if (id != contactType.Id)
            {
                return BadRequest();
            }

            await _bll.ContactTypeService.UpdateAsync(_mapper.Map(contactType)!, User.GetUserId());
            await _bll.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Create new contact type, will belong to the current user
        /// </summary>
        /// <param name="contactType"></param>
        /// <returns></returns>
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(App.DTO.v1.ContactType), StatusCodes.Status200OK)]
        [HttpPost]
        public async Task<ActionResult<App.DTO.v1.ContactType>> PostContactType(App.DTO.v1.ContactTypeCreate contactType)
        {
            var bllContactType = _mapper.Map(contactType);
            _bll.ContactTypeService.Add(bllContactType, User.GetUserId());
            await _bll.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetContactType), new { id = bllContactType.Id }, contactType);
        }

        /// <summary>
        /// Delete contact type by id - owned by current user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteContactType(Guid id)
        {
            await _bll.ContactTypeService.RemoveAsync(id, User.GetUserId());
            await _bll.SaveChangesAsync();
            return NoContent();
        }

    }
}