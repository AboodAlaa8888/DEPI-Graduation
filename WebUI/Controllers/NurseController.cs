using Core.Models;
using Core.RepositoryInterfaces;
using Infrastructure.Data.DbContexts;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebUI.ViewModels;

namespace GraduationProject.Controllers
{
    [Authorize]
    [Route("Nurse")]
    public class NurseController : Controller
    {
        private readonly INurseRepository _nurseRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IWebHostEnvironment hosting;

        public NurseController(INurseRepository nurseRepository, IOrderRepository orderRepository, IWebHostEnvironment Hosting)
        {
            _nurseRepository = nurseRepository;
            _orderRepository = orderRepository;
            hosting = Hosting;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Index")]
        public IActionResult Index([FromQuery] string searchString, [FromQuery] string filterGender, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            var nurses = _nurseRepository.GetAll(searchString, filterGender, pageNumber, pageSize, out int totalRecords);

            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            var viewModel = nurses.Select(n => new NurseViewModel
            {
                Id = n.Id,
                FullName = n.FullName,
                Age = n.Age,
                Experience_years = n.Experience_years,
                Address = n.Address,
                Gender = n.Gender
            }).ToList();

            ViewBag.SearchString = searchString;
            ViewBag.Genders = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Text = "Male", Value = "Male" },
                new SelectListItem { Text = "Female", Value = "Female" }
            }, "Value", "Text");
            ViewBag.FilterGender = filterGender;
            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = totalPages;

            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            return View("/Views/Admin/Index.cshtml", viewModel);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Details/{id:int}")]
        public IActionResult Details([FromRoute] int id)
        {
            var nurse = _nurseRepository.GetById(id);

            if (nurse == null)
                return NotFound();

            var viewModel = new NurseViewModel
            {
                Id = nurse.Id,
                FullName = nurse.FullName,
                Age = nurse.Age,
                Experience_years = nurse.Experience_years,
                Address = nurse.Address,
                Gender = nurse.Gender,
                Description = nurse.Description,
                PictureUrl = nurse.PictureUrl,
                Orders = nurse.Orders
            };

            return View("/Views/Admin/Details.cshtml", viewModel);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Create")]
        public IActionResult Create()
        {
            ViewBag.Genders = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Text = "Male", Value = "Male" },
                new SelectListItem { Text = "Female", Value = "Female" }
            }, "Value", "Text");

            return View("/Views/Account/NurseRegister.cshtml");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Edit/{id:int}")]
        public IActionResult Edit([FromRoute] int id)
        {
            var nurse = _nurseRepository.GetById(id);

            if (nurse == null)
                return NotFound();

            var viewModel = new NurseViewModel
            {
                Id = nurse.Id,
                FullName = nurse.FullName,
                Age = nurse.Age,
                Experience_years = nurse.Experience_years,
                Address = nurse.Address,
                Gender = nurse.Gender,
                Description = nurse.Description,
                PictureUrl = nurse.PictureUrl
            };

            ViewBag.Genders = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Text = "Male", Value = "Male" },
                new SelectListItem { Text = "Female", Value = "Female" }
            }, "Value", "Text");

            return View("/Views/Admin/Edit.cshtml", viewModel);
        }

        [HttpPost("Edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm] NurseViewModel vm)
        {
            if (ModelState.IsValid)
            {
                ModelState.Remove("PictureUrl");
                
                // Get the existing nurse to preserve UserName and other properties
                var existingNurse = _nurseRepository.GetById(vm.Id);
                if (existingNurse == null)
                    return NotFound();

                string fileName = string.Empty;
                if (vm.File != null && vm.File.Length > 0)
                {
                    string uploadsFolder = Path.Combine(hosting.WebRootPath, "uploads");

                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(vm.File.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    await vm.File.CopyToAsync(new FileStream(filePath, FileMode.Create));

                    fileName = uniqueFileName;
                    vm.PictureUrl = "/uploads/" + fileName;
                }
                else
                {
                    // Preserve existing PictureUrl if no new file is uploaded
                    vm.PictureUrl = existingNurse.PictureUrl;
                }

                // Update the existing nurse entity instead of creating a new one
                existingNurse.FullName = vm.FullName;
                existingNurse.Age = vm.Age;
                existingNurse.Experience_years = vm.Experience_years;
                existingNurse.Address = vm.Address;
                existingNurse.Gender = vm.Gender;
                existingNurse.Description = vm.Description;
                existingNurse.PictureUrl = vm.PictureUrl;
                // UserName is preserved from existingNurse

                _nurseRepository.Update(existingNurse);
                TempData["SuccessMessage"] = "Nurse updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Genders = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Text = "Male", Value = "Male" },
                new SelectListItem { Text = "Female", Value = "Female" }
            }, "Value", "Text");

            return View("/Views/Admin/Edit.cshtml", vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Delete/{id:int}")]
        public IActionResult Delete([FromRoute] int id)
        {
            var nurse = _nurseRepository.GetById(id);

            if (nurse == null)
                return NotFound();

            var viewModel = new NurseViewModel
            {
                Id = nurse.Id,
                FullName = nurse.FullName,
                Age = nurse.Age,
                Experience_years = nurse.Experience_years,
                Address = nurse.Address,
                Gender = nurse.Gender
            };

            return View("/Views/Admin/Delete.cshtml", viewModel);
        }

        [HttpPost("Delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed([FromForm] int id)
        {
            var nurse = _nurseRepository.GetById(id);

            if (nurse == null)
                return NotFound();

            _nurseRepository.Delete(nurse);
            TempData["SuccessMessage"] = "Nurse deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("NurseOrdersForAdmin/{nurseId:int}")]
        public IActionResult NurseOrdersForAdmin(int nurseId)
        {
            var nurse = _nurseRepository.GetById(nurseId);
            var orders = _orderRepository.GetOrdersByNurseId(nurseId);

            var viewModel = orders.Select(o => new OrderViewModel
            {
                Id = o.Id,
                Duration = o.Duration,
                Address = o.Address,
                Status = o.Status,
                PatientAge = o.PatientAge,
                OrderDate = o.OrderDate,
                NurseId = o.NurseId
            }).ToList();

            ViewBag.NurseName = nurse.FullName;
            ViewBag.NurseId = nurseId;

            return View("/Views/Admin/NurseOrders.cshtml", viewModel);
        }

        [Authorize(Roles = "Nurse")]
        [HttpGet("Profile")]
        public IActionResult Profile()
        {

            var nurse = _nurseRepository.GetByUserName(User.Identity.Name);

            if (nurse == null)
                return NotFound();

            var viewModel = new NurseViewModel
            {
                Id = nurse.Id,
                FullName = nurse.FullName,
                Age = nurse.Age,
                Experience_years = nurse.Experience_years,
                Address = nurse.Address,
                Gender = nurse.Gender,
                Description = nurse.Description,
                PictureUrl = nurse.PictureUrl,
                Orders = nurse.Orders
            };

            return View("/Views/Nurse/Profile.cshtml", viewModel);
        }


        [Authorize(Roles = "Nurse")]
        [HttpGet("NurseOrdersForNurse/{nurseId:int}")]
        public IActionResult NurseOrdersForNurse(int nurseId)
        {
            var nurse = _nurseRepository.GetById(nurseId);
            var orders = _orderRepository.GetOrdersByNurseId(nurseId);

            var viewModel = orders.Select(o => new OrderViewModel
            {
                Id = o.Id,
                Duration = o.Duration,
                Address = o.Address,
                Status = o.Status,
                PatientAge = o.PatientAge,
                OrderDate = o.OrderDate,
                NurseId = o.NurseId
            }).ToList();

            ViewBag.NurseName = nurse.FullName;
            ViewBag.NurseId = nurseId;

            return View("/Views/Nurse/NurseOrders.cshtml", viewModel);
        }

        [HttpGet("Nurse/ShowOrder/{id}")]
        public IActionResult ShowOrder(int id)
        {
            Order order = _orderRepository.GetById(id);
            return View("ShowOrder", order);
        }

        [HttpPost("Nurse/AcceptOrder/{id}")]
        public IActionResult AcceptOrder(int id)
        {
            Order order = _orderRepository.GetById(id);
            order.Status = OrderStatus.Confirmed;
            _orderRepository.AcceptOrder(order);
            return View("ShowOrder", order);

        }

        [HttpPost("Nurse/CancelOrder/{id}")]
        public IActionResult CancelOrder(int id)
        {
            Order order = _orderRepository.GetById(id);
            order.Status = OrderStatus.Cancelled;
            _orderRepository.CancelOrder(order);
            return View("ShowOrder", order);

        }

        [HttpGet("Sort/Descending")]
        public IActionResult SortDescending()
        {
            var nurses = _nurseRepository.GetAllForUser(null)
                                         .OrderByDescending(n => n.Experience_years)
                                         .ToList();

            var viewModels = nurses.Select(n => new NurseViewModel
            {
                Id = n.Id,
                FullName = n.FullName,
                Age = n.Age,
                Experience_years = n.Experience_years,
                Address = n.Address,
                Gender = n.Gender,
                PictureUrl = n.PictureUrl
            }).ToList();

            return View("/Views/Admin/Index.cshtml", viewModels);
        }

        [HttpGet("Sort/Ascending")]
        public IActionResult SortAscending()
        {
            var nurses = _nurseRepository.GetAllForUser(null)
                                         .OrderBy(n => n.Experience_years)
                                         .ToList();

            var viewModels = nurses.Select(n => new NurseViewModel
            {
                Id = n.Id,
                FullName = n.FullName,
                Age = n.Age,
                Experience_years = n.Experience_years,
                Address = n.Address,
                Gender = n.Gender,
                PictureUrl = n.PictureUrl
            }).ToList();

            return View("/Views/Admin/Index.cshtml", viewModels);
        }
    }
}
