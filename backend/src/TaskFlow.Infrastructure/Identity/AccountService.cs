using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TaskFlow.Application.Abstractions;
using TaskFlow.Application.Auth;

namespace TaskFlow.Infrastructure.Identity
{
    public class AccountService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) : IAccountService
    {
        public async Task<CurrentUserDto?> GetByIdAsync(Guid userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user is null)
            {
                return null;
            }
            var roles = await userManager.GetRolesAsync(user);
            return new CurrentUserDto(user.Id, user.FullName, user.Email ?? string.Empty, roles.ToArray());

            throw new NotImplementedException();
        }

        public async Task<RegistrationResult> RegisterAsync(string fullName, string email, string password)
        {
            var user = new ApplicationUser()
            {
                FullName = fullName.Trim(),
                Email = email.Trim(),
                UserName = email.Trim(),
            };
            var result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                return new RegistrationResult(false, null, result.Errors.Select(e => e.Description).ToArray());
            }
            return new RegistrationResult(true, user.Id, Array.Empty<string>());
        }

        public async Task<bool> SignInAsync(string email, string password)
        {
            var user = await userManager.FindByEmailAsync(email.Trim());
            if (user is null)
            {
                return false;
            }

            var result = await signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: true);

            return result.Succeeded;
        }

        public Task SignOutAsync()
        {
           return signInManager.SignOutAsync();
        }
    }
}