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
        private readonly RegistrationPageDbContext _context;
        private readonly UserManager<RegistrationPageUser> _userManager;
        private readonly SignInManager<RegistrationPageUser> _signInManager;

        public HomeController(ILogger<HomeController> logger, UserManager<RegistrationPageUser> userManager,RegistrationPageDbContext context, SignInManager<RegistrationPageUser> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
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

        public IActionResult Users()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var users = _userManager.Users.Where(x => x.Id != userId);
            return View(users);
        }

        public IActionResult Friends()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var friends = _context.User_Friends.Where(x => x.is_accepted == true && (x.Friend_Id == userId || x.User_Id == userId));
            List<string> friendIds = new List<string>();
            List<RegistrationPageUser> userFriends = new List<RegistrationPageUser>();
            foreach(var friend in friends)
            {
                if(userId == friend.Friend_Id)
                {
                    friendIds.Add(friend.User_Id);
                }
                else
                {
                    friendIds.Add(friend.Friend_Id);
                }
            }
            foreach(var userFriendId in friendIds)
            {
                RegistrationPageUser user = _userManager.FindByIdAsync(userFriendId).Result;
                userFriends.Add(user);
            }
            return View(userFriends);
        }

        public IActionResult Invite(string id)
        {
            var newFriend = new User_Friends();
            newFriend.Friend_Id = id;
            newFriend.User_Id = _userManager.GetUserId(HttpContext.User);
            newFriend.is_accepted = false;
            _context.User_Friends.Add(newFriend);
            _context.SaveChanges();
            return RedirectToAction("Users");
        }

        [HttpGet]
        public IActionResult Invites()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var invites = _context.User_Friends.Where(x => x.is_accepted == false && ( x.Friend_Id == userId || x.User_Id == userId));
            List<string> friendIds = new List<string>();
            List<RegistrationPageUser> userInvites = new List<RegistrationPageUser>();
            foreach (var friend in invites)
            {
                if (userId == friend.Friend_Id)
                {
                    friendIds.Add(friend.User_Id);
                }
                else
                {
                    friendIds.Add(friend.Friend_Id);
                }
            }
            foreach (var userFriendId in friendIds)
            {
                RegistrationPageUser user = _userManager.FindByIdAsync(userFriendId).Result;
                userInvites.Add(user);
            }
            return View(userInvites);
        }

        public IActionResult Accept(string id)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var invite = _context.User_Friends.Where(x => x.User_Id == id && x.Friend_Id == userId).FirstOrDefault();
            invite.is_accepted = true;

            _context.User_Friends.Update(invite);
            _context.SaveChanges();
            return RedirectToAction("Invites");
        }

        public IActionResult Deny(string id)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var invite = _context.User_Friends.Where(x => x.User_Id == id && x.Friend_Id == userId).FirstOrDefault();
            invite.is_accepted = false;

            _context.User_Friends.Remove(invite);
            _context.SaveChanges();
            return RedirectToAction("Invites");
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
