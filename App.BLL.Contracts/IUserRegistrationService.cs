using App.DAL.Contracts;
using App.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace App.BLL.Contracts;

public interface IUserRegistrationService
{
    Task<bool> CreatePersonForUserAsync(AppUser user);
}