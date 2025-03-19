using System.Collections.Generic;
using System.Threading.Tasks;
using App.Business.DTOs;
using App.Business.Services.InternalServices.Interfaces;
using App.Core.Entities.Identity;
using App.Core.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace App.Business.Services.InternalServices.Abstractions
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountService(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public async Task<IdentityResult> CreateUserAsync(  string password, string username )
            {
             

            var user = new User
            {
                UserName = username,
                 EmailConfirmed=true
             };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded) return result;


             await _userManager.AddToRoleAsync(user, EUserRole.Moderator.ToString());
            return IdentityResult.Success;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }
        public async Task<bool> DeleteUserAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return false;

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<List<ShowUserDTO>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var userList = new List<ShowUserDTO>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userList.Add(new ShowUserDTO
                {
                    Username = user.UserName,
                    Role = roles.Any() ? string.Join(", ", roles) : "No Role"
                });
            }

            return userList;
        }



        public async Task<SignInResult> LoginAsync(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return SignInResult.Failed;

            return await _signInManager.PasswordSignInAsync(user, password, true, false);
        }
        public async Task EnsureRolesCreatedAsync()
        {
            foreach (var role in Enum.GetNames(typeof(EUserRole)))
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole()
                    {
                        Name=role
                    });
                }
            }
        }
        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
