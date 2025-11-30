using Core.Models;
using Core.RepositoryInterfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using WebUI.ViewModels;

namespace WebUI.Controllers
{
    [Route("Account")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly INurseRepository _nurseRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IWebHostEnvironment hosting;

        public AccountController(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager,
                                 INurseRepository nurseRepository,
                                 IPatientRepository patientRepository,
                                 IWebHostEnvironment hosting)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _nurseRepository = nurseRepository;
            _patientRepository = patientRepository;
            this.hosting = hosting;
        }

        [HttpGet("Register")]
        public IActionResult Register()
        {
            ViewBag.Genders = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Text = "Male", Value = "Male" },
                new SelectListItem { Text = "Female", Value = "Female" }
            }, "Value", "Text");
            return View("/Views/Account/PatientRegister.cshtml");
        }

        [HttpPost("PatientRegister")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PatientRegister([FromForm] PatientRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user is null)
                {
                    user = new ApplicationUser
                    {
                        FullName = model.FullName,
                        UserName = model.UserName,
                        Email = model.Email,
                        PhoneNumber = model.PhoneNumber,
                        Gender = model.Gender,
                        Age = model.Age,
                        Address = model.Address
                    };

                    var result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "Patient");
                        await _signInManager.SignInAsync(user, isPersistent: false);


                        var patient = new Patient
                        {
                            Id = user.Id,
                            FullName = user.FullName,
                            UserName = user.UserName,
                            Email = user.Email,
                            PhoneNumber = user.PhoneNumber,
                            Gender = user.Gender,
                            Age = user.Age,
                            Address = user.Address
                        };

                        _patientRepository.Add(patient);
                        return RedirectToAction("ShowNurses", "User");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                            ModelState.AddModelError("", error.Description);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty,
                        "This User Name is Exits Already , Please try another one :(");
                }
            }
            ViewBag.Genders = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Text = "Male", Value = "Male" },
                new SelectListItem { Text = "Female", Value = "Female" }
            }, "Value", "Text");
            return View("/Views/Account/PatientRegister.cshtml", model);
        }

        [HttpPost("NurseRegister")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NurseRegister([FromForm] NurseRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                ModelState.Remove("PictureUrl");
                string fileName = string.Empty;
                if (model.File != null && model.File.Length > 0)
                {
                    string uploadsFolder = Path.Combine(hosting.WebRootPath, "uploads");

                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.File.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    await model.File.CopyToAsync(new FileStream(filePath, FileMode.Create));

                    fileName = uniqueFileName;
                    model.PictureUrl = "/uploads/" + fileName;
                }

                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user is null)
                {
                    user = new ApplicationUser
                    {
                        FullName = model.FullName,
                        UserName = model.UserName,
                        Email = model.Email,
                        PhoneNumber = model.PhoneNumber,
                        Gender = model.Gender,
                        Age = model.Age,
                        Address = model.Address
                    };

                    var result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "Nurse");

                        var nurse = new Nurse
                        {
                            FullName = user.FullName,
                            UserName = user.UserName,
                            Age = user.Age,
                            Experience_years = model.Experience_years,
                            Address = user.Address,
                            Gender = user.Gender,
                            Description = model.Description,
                            PictureUrl = model.PictureUrl
                        };

                        _nurseRepository.Add(nurse);
                        TempData["SuccessMessage"] = "Nurse created successfully!";
                        return RedirectToAction("Index", "Nurse");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                            ModelState.AddModelError("", error.Description);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty,
                        "This User Name is Exits Already , Please try another one :(");
                }
            }
            ViewBag.Genders = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Text = "Male", Value = "Male" },
                new SelectListItem { Text = "Female", Value = "Female" }
            }, "Value", "Text");
            return View("/Views/Account/NurseRegister.cshtml", model);
        }

        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View("/Views/Account/Login.cshtml");
        }

        [HttpPost("Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromForm] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user is not null)
                {
                    bool found = await _userManager.CheckPasswordAsync(
                        user, model.Password);

                    if (found)
                    {
                        var result = await _signInManager.PasswordSignInAsync(
                            user, model.Password, model.RememberMe, false);

                        if (result.Succeeded)
                        {
                            if (User.IsInRole("Patient"))
                            {
                                return RedirectToAction("ShowNurses", "User");
                            }

                            if (User.IsInRole("Nurse"))
                            {
                                return RedirectToAction("Profile", "Nurse");
                            }

                            if (User.IsInRole("Admin"))
                            {
                                return RedirectToAction("Index", "Nurse");
                            }
                        }

                        if (result.IsLockedOut)
                        {
                            ModelState.AddModelError(string.Empty,
                                "User account locked out.");
                        }

                        if (result.IsNotAllowed)
                        {
                            ModelState.AddModelError(string.Empty,
                                "Your Account is not allowed");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty,
                            "Incorrect Email Or Password");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid Login");
                }
            }
            return View("/Views/Account/Login.cshtml", model);
        }

        [HttpPost("Logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}
