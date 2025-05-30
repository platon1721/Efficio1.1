// App.BLL.Services/UserRegistrationService.cs
using App.BLL.Contracts;
using App.Domain.Identity;
using Microsoft.Extensions.Logging;

namespace App.BLL.Services;

public class UserRegistrationService : IUserRegistrationService
{
    private readonly IPersonService _personService;
    private readonly ILogger<UserRegistrationService>? _logger;

    public UserRegistrationService(IPersonService personService, ILogger<UserRegistrationService>? logger = null)
    {
        _personService = personService;
        _logger = logger;
    }

    public async Task<bool> CreatePersonForUserAsync(AppUser user)
    {
        try
        {
            var person = await _personService.CreatePersonForUserAsync(
                user.Id, 
                user.FirstName, 
                user.LastName);

            if (person != null)
            {
                _logger?.LogInformation("Successfully created Person for user {UserId}", user.Id);
                return true;
            }

            _logger?.LogWarning("Failed to create Person for user {UserId}", user.Id);
            return false;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to create Person for user {UserId}", user.Id);
            return false;
        }
    }
}