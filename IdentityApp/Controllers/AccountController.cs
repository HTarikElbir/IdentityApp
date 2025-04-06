using IdentityApp.Models;
using IdentityApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Controllers
{
    public class AccountController : Controller
    {
        
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;

        // Constructor
        public AccountController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, SignInManager<AppUser> signInManager) 
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Login action
        public IActionResult Login()
        {
            return View();
        }

        // Login action with model binding
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    await _signInManager.SignOutAsync();
                    
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, true);

                    if (result.Succeeded)
                    {
                        await _userManager.ResetAccessFailedCountAsync(user);
                        await _userManager.SetLockoutEndDateAsync(user, null);

                        return RedirectToAction("Index", "Home");
                    }else if (result.IsLockedOut)
                    {
                        var lockoutDate = await _userManager.GetLockoutEndDateAsync(user);
                        var timeLeft = lockoutDate.Value - DateTime.UtcNow;

                        ModelState.AddModelError("", $"Your account is locked, Please try again after {timeLeft.Minutes} minutes");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid password.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid email.");
                }
                    
            }
            return View(model);
        }
    }
}
