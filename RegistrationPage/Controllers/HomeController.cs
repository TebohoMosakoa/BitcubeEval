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
using RegistrationPage.ViewModels;

namespace RegistrationPage.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<RegistrationPageUser> _userManager;
        private readonly SignInManager<RegistrationPageUser> _signInManager;

        public HomeController(ILogger<HomeController> logger, UserManager<RegistrationPageUser> userManager, SignInManager<RegistrationPageUser> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
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

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            var model = new EditViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName

            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);

            user.Email = model.Email;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;

            var result = await _userManager.UpdateAsync(user);

            if(result.Succeeded)
            {
                return RedirectToAction("Profile");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if(!result.Succeeded)
                {
                    foreach(var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View();
                }
                await _signInManager.RefreshSignInAsync(user);
                return View("PasswordChangeConfirmed");
            }
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
