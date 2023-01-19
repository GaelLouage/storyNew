using Infrastructuur.EnumsAndStaticProps;
using Infrastructuur.extensions;
using Infrastructuur.helpers;
using Infrastructuur.Models;
using Infrastructuur.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
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
        private readonly SpeechSynthesizer _synthesizer;
        private readonly IReviewService _reviewService;
        private readonly IResetTokenService _resetTokenService;
        private readonly IUserSelectedStoryService _userSelectedStoryService;
        private  UserEntity? userConnect;
        public UserController(IUserService userService, ILogger<UserController> logger, SpeechSynthesizer synthesizer, IReviewService reviewService, IUserSelectedStoryService userSelectedStoryService, IResetTokenService resetTokenService)
        {
            _userService = userService;
            _logger = logger;
            _synthesizer = synthesizer;
            _synthesizer.SpeakAsyncCancelAll();
            _reviewService = reviewService;
            _userSelectedStoryService = userSelectedStoryService;
            _resetTokenService = resetTokenService;
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
            if((await _userService?.GetUsersAsync())?.FirstOrDefault(x => x?.Email == HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value) is not null)
            {
                userConnect = (await _userService.GetUsersAsync()).FirstOrDefault(x => x.Email == HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value);
                ViewData["User"] = userConnect;
            }
         
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
            ViewData["User"] = (await _userService.GetUsersAsync()).FirstOrDefault(x => x.Email == HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value);
            var storiesSelected = (await _userSelectedStoryService.GetStoryzonsByUserSelectedIdAsync(id)).ToList();
            ViewData["storiesSelectedByUser"] = storiesSelected;

            return View(await _userService.GetUserByIdAsync(id));
        }
        public async Task<ActionResult> DetailsUser(string userName)
        {
            return RedirectToAction(nameof(Details), new { id = (await _userService.GetUsersAsync()).FirstOrDefault(x => x.Email == HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value).Id });
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
                user.Email = user.Email.ToLower();
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
            ViewData["currentUser"] = (await _userService.GetUsersAsync()).FirstOrDefault(x => x.Email == HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value);
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
                if ((await _userService.GetUsersAsync()).FirstOrDefault(x => x.Email == HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value).Id != userToUpdate.Id)
                {
                    user.Password = userToUpdate.Password.HashToPassword();
                }
                else
                {
                    user.Password = user.Password.HashToPassword();
                }


                await _userService.UpdateUserByIdAsync(id, user);
                var userRole = userConnect;

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
            // Check if the user exists
            if (userLogin is null)
            {
                TempData["ErrorMessage"] = "Wrong username or password.";
                return RedirectToAction(nameof(Login));
            }

            if (!HashPassword.VerifyPassword(user.Password, userLogin.Password))
            {
                TempData["ErrorMessage"] = "Wrong username or password.";
                return RedirectToAction(nameof(Login));
            }

            // Check if the username and password match
            if (!Equals(user.UserName, userLogin.UserName))
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
                }
                else
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
            return RedirectToAction(nameof(Index), "Story");
        }
        //write to excell
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> WriteToExcel()
        {

            var file = (await _userService.GetUsersAsync()).ToList().WriteDataToExcel<UserEntity>("UserData.xls", new Dictionary<string, string>
            {
                {"UserName","UserName" },
                {"FirstName","FirstName" },
                {"Email","Email" },
                {"Role","Role" }
            });
            Response.Headers.Add("Content-Disposition", "attachment; filename=data.xls");


            return File(file.ToArray(), "application/octet-stream");
        }


        // emailsender for password forgot
        // email to send to
        public async Task<IActionResult> ForgotPassword()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var resetEntity = new ResetTokenEntity();
            resetEntity.Email = email;
            await _resetTokenService.AddResetTokenAsync(resetEntity);
            var tokenUser = (await _resetTokenService.GetResetByEmailAsync(email));
            if (tokenUser is null) return View();
            // save the token and expiration date to the database

            //send password to reset email
            string link = $"https://localhost:44316/User/PasswordReset/{tokenUser.Token}";

            var contact = new ContactEntity();
            var message = new MailMessage();
            var smtpClient = new SmtpClient();
            contact.Message = $"{link}";
            contact.UserEmail = email.ToLower();
            var credentials = ReadJson.GetEmailCredentials(@"C:/Users/louag/Desktop/storyContactCredentials/credentials.json");
            contact.EmailAddress = credentials.EmailAddress;
            contact.PassWord = credentials.PassWord;
            // Set the email details
            message = new MailMessage();
            message.From = new MailAddress(contact.UserEmail);
            message.To.Add(contact.EmailAddress);
            message.Subject = "Contact";
            message.Body = contact.Message;

            // Set the server details
            smtpClient = new SmtpClient();
            // use hotmail to send emails
            smtpClient.Host = "smtp.office365.com";
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential(contact.EmailAddress, credentials.PassWord);
            // Send the email
            smtpClient.Send(message);
    
            return View();
        }
        [HttpGet("User/PasswordReset/{token}")]
        public async Task<IActionResult> PasswordReset([FromRoute] string token)
        {
           
             var tokenUser = await _resetTokenService.GetResetByTokenAsync(token);
            if (tokenUser == null) return NotFound();
            (string savedtoken, DateTime expirationtime) = await GetToken(tokenUser.Email);
            if (token == savedtoken && DateTime.Now < expirationtime)
            {
                // token is valid, display the password reset form
                return View();
            }
            else
            {
                // token is invalid or has expired, show an error message
                return View("not found");
            }
        }

        [HttpPost("User/PasswordReset/{token}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PasswordReset([FromRoute]string token, string newPassword, string newPasswordCheck)
        {

            var user = (await _resetTokenService.GetResetByTokenAsync(token));
            if (string.IsNullOrEmpty(newPassword))
            {
                return View();
            }
            if(newPassword != newPasswordCheck)
            {
                return View();
            }
            newPassword = newPassword.HashToPassword();
            if (!(await _userService.UpdatePasswordByEMailAddressAsync(user.Email, newPassword)))
            {
                //give error message not same newpas and newpasscheck or failed
                return View();
            }
            
            await _resetTokenService.RemoveTokenByIdAsync(user.Id);
            return RedirectToAction(nameof(Login));
        }
        public async Task<(string, DateTime)> GetToken(string email)
        {
            // get the token and saved date from the database

            var tokenUser = await _resetTokenService.GetResetByEmailAsync(email);
            return (tokenUser.Token, DateTime.Parse(tokenUser.ExpirationDate));
        }
    }
}
