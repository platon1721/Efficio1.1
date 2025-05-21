using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.BLL.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

// TODO - contact and USER relation via Person and ContactType

namespace WebApp.ApiControllers
{
    /// <inheritdoc />
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly ILogger<ContactsController> _logger;
        private readonly IAppBLL _bll;

        private readonly App.DTO.v1.Mappers.ContactMapper _mapper =
            new App.DTO.v1.Mappers.ContactMapper();

        /// <inheritdoc />
        public ContactsController(IAppBLL bll, ILogger<ContactsController> logger)
        {
            _logger = logger;
            _bll = bll;
        }

        /// <summary>
        /// Get all contacts
        /// </summary>
        /// <returns></returns>
        [Produces("application/json")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<App.DTO.v1.Contact>>> GetContacts()
        {
            return (await _bll.ContactService.AllAsync()).Select(x => _mapper.Map(x)!).ToList();
        }

        /// <summary>
        /// Get contact by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Produces("application/json")]
        [HttpGet("{id}")]
        public async Task<ActionResult<App.DTO.v1.Contact>> GetContact(Guid id)
        {
            var contact = await _bll.ContactService.FindAsync(id);

            if (contact == null)
            {
                return NotFound();
            }

            return _mapper.Map(contact)!;
        }

        /// <summary>
        /// Update contact
        /// </summary>
        /// <param name="id"></param>
        /// <param name="contact"></param>
        /// <returns></returns>
        [Produces("application/json")]
        [Consumes("application/json")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContact(Guid id, App.DTO.v1.Contact contact)
        {
            if (id != contact.Id)
            {
                return BadRequest();
            }

            await _bll.ContactService.UpdateAsync(_mapper.Map(contact)!);
            await _bll.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Create new contact
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        [Produces("application/json")]
        [Consumes("application/json")]
        [HttpPost]
        public async Task<ActionResult<App.DTO.v1.Contact>> PostContact(App.DTO.v1.ContactCreate contact)
        {
            var data = _mapper.Map(contact)!;
            _bll.ContactService.Add(data);
            await _bll.SaveChangesAsync();

            return CreatedAtAction("GetContact", new { id = data.Id }, contact);
        }

        /// <summary>
        /// Delete contact
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Produces("application/json")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(Guid id)
        {
            await _bll.ContactService.RemoveAsync(id);
            await _bll.SaveChangesAsync();

            return NoContent();
        }
    }
}