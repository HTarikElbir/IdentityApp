using IdentityApp.Models;
using IdentityApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace IdentityApp.Controllers
{
    public class UsersController : Controller
    {

        private UserManager<AppUser> _userManager;
        private RoleManager<AppRole> _roleManager;
        public UsersController(UserManager<AppUser> userManager,RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View(_userManager.Users);
        }

        public async Task<IActionResult> Edit(string Id)
        {
            if (Id == null)
            {
                return RedirectToAction("Index");
            }
            var user = await _userManager.FindByIdAsync(Id);

            if (user != null)
            {
                ViewBag.Roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
                return View(new EditViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    SelectedRoles = await _userManager.GetRolesAsync(user),
                });
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string Id, EditViewModel model)
        {
            if (Id != model.Id)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);

                if (user != null)
                {
                    user.Email = model.Email;
                    user.FullName = model.FullName;

                    var result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded && !string.IsNullOrEmpty(model.Password))
                    {
                        await _userManager.RemovePasswordAsync(user);
                        await _userManager.AddPasswordAsync(user, model.Password);
                    }

                    if (result.Succeeded)
                    {
                        await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));

                        if (model.SelectedRoles != null)
                        {
                            await _userManager.AddToRolesAsync(user, model.SelectedRoles);
                        }

                        return RedirectToAction("Index");
                    }

                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string Id)
        {
            if (!string.IsNullOrEmpty(Id))
            {
                var user = await _userManager.FindByIdAsync(Id);
                if (user != null)
                {
                    var result = await _userManager.DeleteAsync(user);
                   
                }
            }

            return RedirectToAction("Index");
        }
    }
}
