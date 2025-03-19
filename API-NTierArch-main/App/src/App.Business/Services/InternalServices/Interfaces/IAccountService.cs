using System.Collections.Generic;
using System.Threading.Tasks;
using App.Business.DTOs;
using App.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace App.Business.Services.InternalServices.Interfaces
{
    public interface IAccountService
    {
        Task<IdentityResult> CreateUserAsync(string username, string password);
        Task<User> GetUserByEmailAsync(string email);
        Task<List<ShowUserDTO>> GetAllUsersAsync();
        
            Task<bool> DeleteUserAsync(string username);
         

        Task<SignInResult> LoginAsync(string email, string password);
        Task LogoutAsync();   Task EnsureRolesCreatedAsync();
    }
}
