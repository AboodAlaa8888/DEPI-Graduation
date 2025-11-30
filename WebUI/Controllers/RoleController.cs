using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebUI.ViewModels;

namespace WebUI.Controllers
{
    [Authorize]
    [Route("Role")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Index")]
        public IActionResult Index()
        {
            var roles = _roleManager.Roles.ToList();

            var viewModel = roles.Select(role => new RoleViewModel
            {
                Id = role.Id,
                Name = role.Name
            }).ToList();

            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            return View("/Views/Role/Index.cshtml", viewModel);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View("/Views/Role/Create.cshtml");
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var roleExists = await _roleManager.RoleExistsAsync(model.Name);

                if (!roleExists)
                {
                    var role = new IdentityRole(model.Name);
                    var result = await _roleManager.CreateAsync(role);

                    if (result.Succeeded)
                    {
                        TempData["SuccessMessage"] = "Role created successfully!";
                        return RedirectToAction("Index", "Role");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                            ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "This role already exists!");
                }
            }

            return View("/Views/Role/Create.cshtml", model);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("Edit")]
        public async Task<IActionResult> Edit([FromQuery] string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
                return NotFound();

            var viewModel = new RoleViewModel
            {
                Id = role.Id,
                Name = role.Name
            };

            return View("/Views/Role/Edit.cshtml", viewModel);
        }

        [HttpPost("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm] RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(model.Id);

                if (role == null)
                    return NotFound();

                role.Name = model.Name;

                var result = await _roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Role updated successfully!";
                    return RedirectToAction("Index", "Role");
                }
                else
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View("/Views/Role/Edit.cshtml", model);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Delete")]
        public async Task<IActionResult> Delete([FromQuery] string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
                return NotFound();

            var viewModel = new RoleViewModel
            {
                Id = role.Id,
                Name = role.Name
            };

            return View("/Views/Role/Delete.cshtml", viewModel);
        }

        [HttpPost("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed([FromForm] string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
                return NotFound();

            var result = await _roleManager.DeleteAsync(role);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Role deleted successfully!";
                return RedirectToAction("Index", "Role");
            }
            else
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }

            return View("/Views/Role/Delete.cshtml", role);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> AddOrRemoveUser(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role is null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = roleId;
            var userInRoleDto = new List<UserRolesViewModel>();
            var users = await _userManager.Users.ToListAsync();
            foreach (var user in users)
            {
                var userInRole = new UserRolesViewModel()
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                };
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userInRole.IsSelected = true;
                }
                else
                {
                    userInRole.IsSelected = false;
                }
                userInRoleDto.Add(userInRole);
            }
            return View(userInRoleDto);
        }

        [HttpPost]
        public async Task<IActionResult> AddOrRemoveUser(string roleId, List<UserRolesViewModel> users)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role is null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                foreach (var user in users)
                {
                    var appUser = await _userManager.FindByIdAsync(user.UserId);
                    if (appUser is not null)
                    {
                        if (user.IsSelected && !await _userManager.IsInRoleAsync(appUser, role.Name))
                        {
                            await _userManager.AddToRoleAsync(appUser, role.Name);
                        }
                        else if (!user.IsSelected && await _userManager.IsInRoleAsync(appUser, role.Name))
                        {
                            await _userManager.RemoveFromRoleAsync(appUser, role.Name);
                        }
                    }
                }
                return RedirectToAction(nameof(Edit), new { id = roleId });
            }
            return View(users);
        }
    }

}
