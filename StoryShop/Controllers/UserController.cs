using Infrastructuur.EnumsAndStaticProps;
using Infrastructuur.extensions;
using Infrastructuur.Models;
using Infrastructuur.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace StoryShop.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        // GET: UserController
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Index()
        {
            return View((await _userService.GetUsersAsync()).ToList());
        }
        // GET: UserController
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> UserManagement()
        {
            return View((await _userService.GetUsersAsync()).ToList());
        }
        // GET: UserController/Details/5
        public async Task<ActionResult> Details(string id)
        {
            return View(await _userService.GetUserByIdAsync(id));
        }

        // GET: UserController/Create
        [Authorize(Roles = "Admin,SuperAdmin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserController/Create
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(UserEntity user)
        {
            try
            {
                await _userService.AddUserAsync(user);
                return RedirectToAction(nameof(UserManagement));
            }
            catch
            {
                return View();
            }
        }

        // GET: UserController/Edit/5
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ActionResult> Edit(string id)
        {
            return View(await _userService.GetUserByIdAsync(id));
        }

        // POST: UserController/Edit/5
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, UserEntity user)
        {
            try
            {
                await _userService.UpdateUserByIdAsync(id, user);
                return RedirectToAction(nameof(UserManagement));
            }
            catch
            {
                return View();
            }
        }
        [Authorize(Roles = "Admin,SuperAdmin")]
        // GET: UserController/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            return View(await _userService.GetUserByIdAsync(id));
        }

        // POST: UserController/Delete/5
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id, UserEntity user)
        {
            try
            {
                await _userService.DeleteUserByIdAsync(id);
                return RedirectToAction(nameof(UserManagement));
            }
            catch
            {
                return View();
            }
        }

        // login
        public IActionResult Login()
        {
            return View();
        }
        public async Task<IActionResult> ValidateLogin(UserEntity user)
        {
            // Check if the username or password is empty
            if (string.IsNullOrEmpty(user.Password) || string.IsNullOrEmpty(user.UserName))
            {
                TempData["ErrorMessage"] = "Wrong username or password.";
                return RedirectToAction(nameof(Login));
            }

            // Hash the password
            user.Password = user.Password.HashToPassword();

            // Get the user from the database
            var userLogin = await _userService.GetUserByNameAndPasswordAsync(user.UserName, user.Password);

            // Check if the user exists
            if (userLogin is null)
            {
                TempData["ErrorMessage"] = "Wrong username or password.";
                return RedirectToAction(nameof(Login));
            }

            // Check if the username and password match
            if (!Equals(user,userLogin))
            {
                TempData["ErrorMessage"] = "Wrong username or password.";
                return RedirectToAction(nameof(Login));
            }
            // add app.UseAuthentication(); to program.cs
            // Create a list of claims
            var claims = new List<Claim>
                     {
                         new Claim("UserName", userLogin.UserName),
                         new Claim(ClaimTypes.Name, userLogin.UserName),
                         new Claim("Email", userLogin.Email),
                         new Claim(ClaimTypes.Email, userLogin.Email),
                          new Claim(userLogin.Role, userLogin.Role),
                         new Claim(ClaimTypes.Role, userLogin.Role)
                     };

            // Create a ClaimsIdentity and ClaimsPrincipal
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Sign in the user
            await HttpContext.SignInAsync(claimsPrincipal);

            // Redirect to the index page
            return RedirectToAction(nameof(Index), "Story");
        }
        // regiser
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ValidateRegister(UserEntity user, string confirm_password)
        {
            // Check if the passwords match
            if (user.Password != confirm_password)
            {
                TempData["ErrorMessage"] = "Passwords not the same.";
                return RedirectToAction(nameof(Register));
            }

            // Hash the password
            user.Password = user.Password.HashToPassword();

            // Try to add the user to the database
            try
            {
                if (!await _userService.AddUserAsync(user))
                {
                    TempData["ErrorMessage"] = "A user with those credentials already exists.";
                    return RedirectToAction(nameof(Register));
                } else
                {
                    return RedirectToAction(nameof(Login));
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex.ToString());
                TempData["ErrorMessage"] = "An unexpected error occurred.";
                return RedirectToAction(nameof(Register));
            }
        }
        //[Authorize(Roles = "Admin,SuperAdmin,User")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {

            await HttpContext.SignOutAsync();
            return RedirectToAction(nameof(Index),"Story");
        }
        //write to excell
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> WriteToExcel()
        {

            if ((await _userService.GetUsersAsync()).ToList().WriteDataToExcel<UserEntity>("UserData.xls", new Dictionary<string, string>
            {
                {"UserName","UserName" },
                {"FirstName","FirstName" },
                {"LastName","Email" },
                {"Role","Role" }
            }))
            {
                return RedirectToAction(nameof(UserManagement));
            }
            return RedirectToAction(nameof(UserManagement));
        }
    }
}
