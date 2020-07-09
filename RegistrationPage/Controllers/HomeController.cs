using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RegistrationPage.Areas.Identity.Data;
using RegistrationPage.Models;

namespace RegistrationPage.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<RegistrationPageUser> _userManager;
        

        public HomeController(ILogger<HomeController> logger, UserManager<RegistrationPageUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Profile()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            RegistrationPageUser user = _userManager.FindByIdAsync(userId).Result;
            return View(user);
        }
        public IActionResult Friends()
        {
            var users = _userManager.Users; 
            return View(users);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
