using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Abstractions;
using TaskFlow.Application.Auth;
using TaskFlow.Application.Common.Security;
using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Infrastructure.Identity
{
    public class AccountService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, TaskFlowDbContext dbContext) : IAccountService
    {
        public async Task<CurrentUserDto?> GetByIdAsync(Guid userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user is null)
            {
                return null;
            }
            var roles = await userManager.GetRolesAsync(user);
            return new CurrentUserDto(user.Id, user.FullName, user.Email ?? string.Empty, roles.ToArray(), user.TeamMemberId);

            throw new NotImplementedException();
        }

        public async Task<RegistrationResult> RegisterAsync(string fullName, string email, string password)
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();
            await using var transaction = await dbContext.Database.BeginTransactionAsync();
            
            var member = await dbContext.TeamMembers.SingleOrDefaultAsync(
           member => member.Email.ToLower() == normalizedEmail);
         
            if (member is null)
            {
                member = new TeamMember
                {
                    Id = Guid.NewGuid(),
                    FullName = fullName.Trim(),
                    Email = normalizedEmail
                };

                dbContext.TeamMembers.Add(member);
            }

            var user = new ApplicationUser()
            {
                FullName = fullName.Trim(),
                Email = email.Trim(),
                UserName = email.Trim(),
            };
            var createResult = await userManager.CreateAsync(user, password);

            if (!createResult.Succeeded)
            {
                return new RegistrationResult(
                    false,
                    null,
                    createResult.Errors
                        .Select(error => error.Description)
                        .ToArray());
            }
            var roleResult = await userManager.AddToRoleAsync(user, AppRoles.Employee);

            if (!roleResult.Succeeded)
            {
                return new RegistrationResult(
                    false,
                    null,
                    roleResult.Errors
                        .Select(error => error.Description)
                        .ToArray());
            }

            await transaction.CommitAsync();

            return new RegistrationResult(
                true,
                user.Id,
                Array.Empty<string>());
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