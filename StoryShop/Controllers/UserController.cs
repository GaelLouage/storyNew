using Infrastructuur.EnumsAndStaticProps;
using Infrastructuur.extensions;
using Infrastructuur.helpers;
using Infrastructuur.Models;
using Infrastructuur.Services.Interfaces;
using Infrastructuur.singleton;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Speech.Synthesis;
using System.Text;

namespace StoryShop.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        private readonly UserSingleton _userSingleton;
        private readonly SpeechSynthesizer _synthesizer;
        private readonly IReviewService _reviewService;
        public UserController(IUserService userService, ILogger<UserController> logger, UserSingleton userSingleton, SpeechSynthesizer synthesizer, IReviewService reviewService)
        {
            _userService = userService;
            _logger = logger;
            _userSingleton = userSingleton;
            _synthesizer = synthesizer;
            _synthesizer.SpeakAsyncCancelAll();
            _reviewService = reviewService;
        }

        // GET: UserController
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Index()
        {
            return View((await _userService.GetUsersAsync()).ToList());
        }
        // GET: UserController
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> UserManagement(string filtering, string searchInput)
        {
            ViewData["User"] = _userSingleton.User;
            var users = (await _userService.GetUsersAsync()).ToList();
            if (!string.IsNullOrEmpty(searchInput))
            {
                users = users.Where(x => x.FirstName.ToLower().Contains(searchInput.ToLower())
                || x.LastName.ToLower().Contains(searchInput.ToLower()) 
                || x.UserName.ToLower().Contains(searchInput.ToLower())
                || x.Email.ToLower().Contains(searchInput.ToLower())).ToList();
            }
           
            switch (filtering)
            {
                case "UserName":
                    users = users.OrderBy(x => x.UserName).ToList();
                    break;
                case "FirstName":
                    users = users.OrderBy(x => x.FirstName).ToList();
                    break;
                case "LastName":
                    users = users.OrderBy(x => x.LastName).ToList();
                    break;
                case "Email":
                    users = users.OrderBy(x => x.Email).ToList();
                    break;
                case "Role":
                    users = users.OrderBy(x => x.Role).ToList();
                    break;
            }
          
            return View(users);
        }
        // GET: UserController/Details/5
        public async Task<ActionResult> Details(string id)
        {
            ViewData["User"] = _userSingleton.User;
            return View(await _userService.GetUserByIdAsync(id));
        }
        public async Task<ActionResult> DetailsUser(string userName)
        {
            return RedirectToAction(nameof(Details), new { id = _userSingleton.User.Id });
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
                user.Password = user.Password.HashToPassword();
                await _userService.AddUserAsync(user);
                return RedirectToAction(nameof(UserManagement));
            }
            catch
            {
                return View();
            }
        }

        // GET: UserController/Edit/5
        [Authorize(Roles = "Admin,SuperAdmin,User")]
        public async Task<ActionResult> Edit(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            ViewData["currentUser"] = _userSingleton.User;
            return View(await _userService.GetUserByIdAsync(id));
        }

        // POST: UserController/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, UserEntity user)
        {
            try
            {
                var userToUpdate = await _userService.GetUserByIdAsync(id);
                if(_userSingleton.User.Id != userToUpdate.Id)
                {
                    user.Password = userToUpdate.Password.HashToPassword();
                } else
                {
                    user.Password = user.Password.HashToPassword();
                }
             

                await _userService.UpdateUserByIdAsync(id, user);
                var userRole = _userSingleton.User;

                if (userRole is not null && (userRole.Role == Role.Admin || userRole.Role == Role.SuperAdmin))
                {
                    return RedirectToAction(nameof(UserManagement));
                }
                return RedirectToAction(nameof(Index), "Story");
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
                var reviews = (await _reviewService.GetReviews())
                    .Where(x => x.UserId == id);
           
                foreach (var review in reviews)
                {
                    await _reviewService.RemoveReview(review.ReviewId);
                }
              
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

           
            // Get the user from the database
            var userLogin = await _userService.GetUserByNameAsync(user.UserName);
            if (!HashPassword.VerifyPassword(user.Password, userLogin.Password))
            {
                TempData["ErrorMessage"] = "Wrong username or password.";
                return RedirectToAction(nameof(Login));
            }
                // Check if the user exists
                if (userLogin is null)
            {
                TempData["ErrorMessage"] = "Wrong username or password.";
                return RedirectToAction(nameof(Login));
            }

            // Check if the username and password match
            if (!Equals(user.UserName,userLogin.UserName))
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

        //public async Task<IActionResult> PasswordReseter(string email)
        //{
        //    var userByEmail = (await _userService.GetUsersAsync())
        //        .FirstOrDefault(x => x.Email == email);
    
        //    var credentials = ReadJson.GetEmailCredentials(@"C:/Users/louag/Desktop/storyContactCredentials/credentials.json");
        
        //    MailMessage message = new MailMessage();
        //    message.From = new MailAddress(credentials.EmailAddress);
        //    message.To.Add(email);
        //    message.Subject = "password";
        //    message.Body = userByEmail.Password;

        //    // Set the server details
        //    SmtpClient smtpClient = new SmtpClient();
        //    smtpClient.Host = "smtp.office365.com";
        //    smtpClient.Port = 587;
        //    smtpClient.EnableSsl = true;
        //    smtpClient.Credentials = new NetworkCredential(credentials.EmailAddress, credentials.PassWord);
        //    return RedirectToAction(nameof(Login));
        //}
    }
}
