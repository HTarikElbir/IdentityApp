using IdentityApp.Models;
using IdentityApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Controllers
{
    public class AccountController : Controller
    {
        
        private UserManager<AppUser> _userManager;
        private RoleManager<AppRole> _roleManager;
        private SignInManager<AppUser> _signInManager;
        private IEmailSender _emailSender;
        public AccountController(
            UserManager<AppUser> userManager, 
            RoleManager<AppRole> roleManager,
            SignInManager<AppUser> signInManager,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _roleManager = roleManager; 
            _signInManager = signInManager;
            _emailSender = emailSender;
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
                    
                   
                    if(!await _userManager.IsEmailConfirmedAsync(user))
                    {
                        ModelState.AddModelError("", "Email is not confirmed");
                        return View(model);
                    }

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


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    FullName = model.FullName,
                };

                IdentityResult result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Send confirmation email
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var url = Url.Action("ConfirmEmail", "Account", new { user.Id, token });

                    await _emailSender.SendEmailAsync(user.Email, "Confirm", "Test");
                    TempData["Message"] = "Registration successful. Please check your email to confirm your account.";
                    return RedirectToAction("Login", "Account");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> ConfirmEmail(string Id, string token)
        {
            if(Id == null || token == null)
            {
                TempData["Message"] = "Invalid token information.";
                return View();
            }

            var user = await _userManager.FindByIdAsync(Id);

            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);

                if (result.Succeeded)
                {
                    TempData["Message"] = "Email confirmed successfully.";
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    TempData["Message"] = "Email confirmation failed.";
                    return View();
                }
            }
            TempData["Message"] = "User not found.";
            return View();
        }
    }
}
