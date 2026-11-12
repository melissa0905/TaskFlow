using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Application.Common.Security;

namespace TaskFlow.Infrastructure.Identity
{
    public static class IdentitySeeder
    {
        public static async Task InitializeAsync(
        IServiceProvider services,
            string? adminEmail,
            string? adminPassword,
            string? adminFullName)
        {
            await using var scope = services.CreateAsyncScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            foreach (var roleName in new[]
            {
                AppRoles.Admin,
                AppRoles.Employee
            })
            {
                if (await roleManager.RoleExistsAsync(roleName))
                {
                    continue;
                }
                var role = new IdentityRole<Guid>(roleName)
                {
                    Id = Guid.NewGuid()
                };
                EnsureSucceeded(
              await roleManager.CreateAsync(role));
                if (string.IsNullOrWhiteSpace(adminEmail))
                {
                    return;
                }
                var admin = await userManager.FindByEmailAsync(
           adminEmail.Trim());
                if (admin is null)
                {
                    if (string.IsNullOrWhiteSpace(adminPassword))
                    {
                        throw new InvalidOperationException(
                            "İlk admin hesabı için BootstrapAdmin:Password gereklidir.");
                    }
                    admin = new ApplicationUser
                    {
                        Email = adminEmail.Trim(),
                        UserName = adminEmail.Trim(),
                        FullName = string.IsNullOrWhiteSpace(adminFullName)
                        ? "TaskFlow Yöneticisi"
                        : adminFullName.Trim()
                    };

                    EnsureSucceeded(
                        await userManager.CreateAsync(
                            admin,
                            adminPassword));
                }
                if (!await userManager.IsInRoleAsync(admin,AppRoles.Admin))
                {
                    EnsureSucceeded(
                        await userManager.AddToRoleAsync(
                            admin,
                            AppRoles.Admin));
                }
            }
        }

        private static void EnsureSucceeded(IdentityResult result)
        {
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    string.Join(
                        "; ",
                        result.Errors.Select(error => error.Description)));
            }
        }
    }
}