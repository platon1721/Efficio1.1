using System.Security.Claims;
using App.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.DataSeeding;

public static class AppDataInit
{
    public static void SeedAppData(AppDbContext context)
    {
    }

    public static void MigrateDatabase(AppDbContext context)
    {
        context.Database.Migrate();
    }

    public static void DeleteDatabase(AppDbContext context)
    {
        context.Database.EnsureDeleted();
    }

    public static void SeedIdentity(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        foreach (var (roleName, id) in InitialData.Roles)
        {
            var role = roleManager.FindByNameAsync(roleName).Result;

            if (role != null) continue;

            role = new AppRole()
            {
                Id = id ?? Guid.NewGuid(),
                Name = roleName,
            };

            var result = roleManager.CreateAsync(role).Result;
            if (!result.Succeeded)
            {
                throw new ApplicationException("Role creation failed!");
            }
        }


        foreach (var userInfo in InitialData.Users)
        {
            var user = userManager.FindByEmailAsync(userInfo.name).Result;
            if (user == null)
            {
                user = new AppUser()
                {
                    Id = userInfo.id ?? Guid.NewGuid(),
                    Email = userInfo.name,
                    UserName = userInfo.name,
                    EmailConfirmed = true,
                    FirstName = userInfo.firstName,
                    LastName = userInfo.lastName,
                };
                var result = userManager.CreateAsync(user, userInfo.password).Result;
                if (!result.Succeeded)
                {
                    throw new ApplicationException("User creation failed!");
                }

                result = userManager.AddClaimAsync(user, new Claim(ClaimTypes.GivenName, user.FirstName)).Result;
                if (!result.Succeeded)
                {
                    throw new ApplicationException("Claim adding failed!");
                }
                result = userManager.AddClaimAsync(user, new Claim(ClaimTypes.Surname, user.LastName)).Result;
                if (!result.Succeeded)
                {
                    throw new ApplicationException("Claim adding failed!");
                }
            }

            foreach (var role in userInfo.roles)
            {
                if (userManager.IsInRoleAsync(user, role).Result)
                {
                    Console.WriteLine($"User {user.UserName} already in role {role}");
                    continue;
                }

                var roleResult = userManager.AddToRoleAsync(user, role).Result;
                if (!roleResult.Succeeded)
                {
                    foreach (var error in roleResult.Errors)
                    {
                        Console.WriteLine(error.Description);
                    }
                }
                else
                {
                    Console.WriteLine($"User {user.UserName} added to role {role}");
                }
            }
        }
    }
}