using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskFlow.Application.Auth;

namespace TaskFlow.Application.Abstractions
{
    public interface IAccountService
    {
        Task<RegistrationResult> RegisterAsync(string fullName,string email,string password);
        Task<bool> SignInAsync(string email,string password);
        Task<CurrentUserDto?> GetByIdAsync(Guid userId);
        Task SignOutAsync(); 
    }
}