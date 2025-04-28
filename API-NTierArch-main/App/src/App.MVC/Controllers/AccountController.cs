using System.Threading.Tasks;
using App.Business.DTOs;
using App.Business.Services.InternalServices.Interfaces;
using App.Core.Entities.Identity;
using App.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace App.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(IAccountService accountService, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _accountService = accountService;
            this.userManager = userManager;
            _roleManager = roleManager;
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ShowAllUsers()
        {
            var users = await _accountService.GetAllUsersAsync();
            return View(users);
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginUserDTO loginUserDTO)
        {
           
            var result = await _accountService.LoginAsync(loginUserDTO.Username, loginUserDTO.Password);
            if (result.Succeeded)
            {
                Console.WriteLine($"asddsasaadssda");
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Daxil etdiyiniz məlumatlar səhvdir");

            return View(loginUserDTO);  
        }
        public async Task<IActionResult> Test()
        {
            var newUser = new User
            {
                UserName = "ismayil1",
                Email = "ismayil2@gmail.com",
                EmailConfirmed = true,
            };

             var result = await userManager.CreateAsync(newUser, "Ii05182609*");

             if (result.Succeeded)
            {
                 var addToRoleResult = await userManager.AddToRoleAsync(newUser, EUserRole.Admin.ToString());

                if (addToRoleResult.Succeeded)
                {
                    Console.WriteLine("User created and added to role successfully!");
                }
                else
                {
                     foreach (var error in addToRoleResult.Errors)
                    {
                        Console.WriteLine($"Role Assignment Error: {error.Description}");
                    }
                }
            }
            else
            {
                 foreach (var error in result.Errors)
                {
                    Console.WriteLine($"User Creation Error: {error.Description}");
                }
            }

            return View(newUser);
        }


        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutAsync();
            return RedirectToAction("Login");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser(CreateUserDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _accountService.CreateUserAsync(dto.Password, dto.Username);
            if (result.Succeeded)
                return RedirectToAction("Index","Home");

            foreach (var error in result.Errors)
            {
                var translatedMessage = TranslateError(error.Code);
                ModelState.AddModelError("", translatedMessage);
            }

            return View(dto);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser([FromBody] DeleteUserRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Username))
            {
                return Json(new { success = false, message = "İstifadəçi adı boş ola bilməz." });
            }

            var success = await _accountService.DeleteUserAsync(request.Username);
            if (success)
                return Json(new { success = true });

            return Json(new { success = false, message = "İstifadəçi silinə bilmədi." });
        }


        public async Task<IActionResult> CreateRoles()
        {
            foreach (var item in typeof(EUserRole).GetEnumValues())
            {
                if (!await _roleManager.RoleExistsAsync(item.ToString()))
                {
                    await _roleManager.CreateAsync(new IdentityRole()
                    {
                        Name = item.ToString(),

                    });
                }
            }
            return Json(new { success = true });
        }
        public class DeleteUserRequest
        {
            public string Username { get; set; }
        }

        private string TranslateError(string errorCode)
        {
            return errorCode switch
            {
                "PasswordTooShort" => "Şifrə çox qısadır.",
                "PasswordRequiresNonAlphanumeric" => "Şifrə ən azı bir xüsusi simvol daxil etməlidir.",
                "PasswordRequiresDigit" => "Şifrə ən azı bir rəqəm daxil etməlidir.",
                "PasswordRequiresLower" => "Şifrə ən azı bir kiçik hərf daxil etməlidir.",
                "PasswordRequiresUpper" => "Şifrə ən azı bir böyük hərf daxil etməlidir.",
                "DuplicateUserName" => "Bu istifadəçi adı artıq mövcuddur.",
                "DuplicateEmail" => "Bu e-poçt ünvanı artıq mövcuddur.",
                _ => "Naməlum xəta baş verdi."
            };
        }
    }
}
