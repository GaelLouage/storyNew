using Infrastructuur.helpers;
using Infrastructuur.Models;
using Microsoft.AspNetCore.Mvc;
using StoryShop.Models;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using Infrastructuur.helpers;
namespace StoryShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private static bool mailIsSuccess = false;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Contact()
        {
            mailIsSuccess = false;
            return View();
        }
        [HttpPost]
        public IActionResult Contact(ContactEntity contact)
        {
            try
            {
                TempData["email"] = "";
                MailMessage message;
                SmtpClient smtpClient;
                Email.SendEmail(contact, out message, out smtpClient);
                mailIsSuccess = true;
                if (mailIsSuccess)
                {
                    TempData["email"] = "Email Succesfully send!";
                }
              
                return View();
            }
            catch
            {
                return View();
            }
      
        }

      
    }
}