using Infrastructuur.Models;
using Microsoft.AspNetCore.Mvc;
using StoryShop.Models;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace StoryShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
      
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
      
    }
}