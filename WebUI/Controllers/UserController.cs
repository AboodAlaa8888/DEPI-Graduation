using Core.Models;
using Core.RepositoryInterfaces;
using Infrastructure.Data.DbContexts;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using WebUI.ViewModels;

namespace GraduationProject.Controllers
{
    [Authorize]
    [Route("User")]
    public class UserController : Controller
    {
        private readonly INurseRepository _nurseRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IPatientRepository _patientRepository;

        public UserController(INurseRepository nurseRepository, IOrderRepository orderRepository, IPatientRepository patientRepository)
        {
            _nurseRepository = nurseRepository;
            _orderRepository = orderRepository;
            _patientRepository = patientRepository;
        }

        [Authorize(Roles = "Patient")]
        [HttpGet("GetPatient")]
        public IActionResult GetPatient(string id)
        {
            var patient = _patientRepository.GetById(id);

            return View(patient);
        }

        [Authorize(Roles = "Patient")]
        [HttpGet("ShowNurses")]
        public IActionResult ShowNurses(string genderString)
        {
            var nurses = _nurseRepository.GetAllForUser(genderString);


            var viewModel = nurses.Select(n => new NurseViewModel
            {
                Id = n.Id,
                FullName = n.FullName,
                Age = n.Age,
                Experience_years = n.Experience_years,
                Address = n.Address,
                Gender = n.Gender,
                Description = n.Description,
                PictureUrl = n.PictureUrl
            }).ToList();

            ViewBag.Genders = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Text = "All", Value = "All" },
                new SelectListItem { Text = "Male", Value = "Male" },
                new SelectListItem { Text = "Female", Value = "Female" }
            }, "Value", "Text");

            return View("/Views/User/ShowNurses.cshtml", viewModel);
        }

        [Authorize(Roles = "Patient")]
        [HttpGet("Nurse/{nurseId:int}")]
        public IActionResult Nurse(int nurseId)
        {
            var nurse = _nurseRepository.GetById(nurseId);

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

            return View("/Views/User/Nurse.cshtml", viewModel);
        }

        [Authorize(Roles = "Patient")]
        [HttpGet("CreateOrder/{nurseId:int}")]
        public IActionResult CreateOrder(int nurseId)
        {
            ViewBag.NurseId = nurseId;
            ViewBag.PatientId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            ViewBag.Genders = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Text = "Male", Value = "Male" },
                new SelectListItem { Text = "Female", Value = "Female" }
            }, "Value", "Text");

            return View("/Views/User/CreateOrder.cshtml");
        }

        [HttpPost("CreateOrder/{id:int}")]
        public IActionResult CreateOrder([FromRoute] int id, [FromForm] OrderViewModel vm)
        {

            if (ModelState.IsValid)
            {

                // DateTime selectedDate = vm.OrderDate;
                var order = new Order
                {
                    Description = vm.Description,
                    Duration = vm.Duration,
                    Address = vm.Address,
                    PatientAge = vm.PatientAge,
                    NurseId = vm.NurseId,
                    PatientId = vm.PatientId,
                    OrderDate = vm.OrderDate,
                    Gender = vm.Gender
                };
                //  DateTime? IsConflict = _orderRepository.IsConflict(order);
                //var endTime = vm.OrderDate.AddHours(vm.Duration);


                Order? flag = _orderRepository.IsConflict(order);

                if (flag != null)
                {
                    //var endTime = IsConflict.AddHours(vm.Duration);
                    // var conflictStart = flag.OrderDate.ToString("HH");
                    var conflictEnd = flag.OrderDate.AddHours(flag.Duration);
                    //  ModelState.AddModelError("", $"{vm.OrderDate:HH:mm}لا يمكن حجز هذه الخدمة في هذا الوقت. هناك حجز آخر متداخل.");
                    ModelState.AddModelError("", $"لا يمكن حجز هذه الخدمة في هذا الوقت. ({flag.OrderDate:HH:mm} - {conflictEnd:HH:mm})");
                    ViewBag.PatientId = vm.PatientId;

                    ViewBag.Genders = new SelectList(new List<SelectListItem>
            {
                     new SelectListItem { Text = "Male", Value = "Male" },
                     new SelectListItem { Text = "Female", Value = "Female" }
                }, "Value", "Text");
                    ViewBag.NurseId = vm.NurseId;
                    return View(vm);
                }
                _orderRepository.Add(order);

                return RedirectToAction(nameof(ShowNurses));
            }

            ViewBag.NurseId = vm.NurseId;

            //ViewBag.Durations = new SelectList(new List<SelectListItem>
            //{
            //    new SelectListItem { Text = "3 Hours", Value = "3 Hours" },
            //    new SelectListItem { Text = "6 Hours", Value = "6 Hours" },
            //    new SelectListItem { Text = "9 Hours", Value = "9 Hours" }
            //}, "Value", "Text");






            return View("/Views/User/CreateOrder.cshtml", vm);
        }

        [Route("SortAscending")]
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

            return View("ShowNurses", viewModels);
        }

        [Route("SortDescending")]

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

            return View("ShowNurses", viewModels);
        }



        [Authorize(Roles = "Patient")]
        [HttpGet("Profile")]
        public IActionResult Profile()
        {
            var patient = _patientRepository.GetByUserName(User.Identity.Name);

            if (patient == null)
                return NotFound();

            var viewModel = new PatientViewModel
            {
                Id = patient.Id,
                FullName = patient.FullName,
                UserName = patient.UserName,
                Email = patient.Email,
                PhoneNumber = patient.PhoneNumber,
                Gender = patient.Gender,
                Age = patient.Age,
                Address = patient.Address,
                Orders = patient.Orders
            };

            return View("/Views/User/Profile.cshtml", viewModel);
        }

        [Authorize(Roles = "Patient")]
        [HttpGet("PatientOrders")]
        public IActionResult PatientOrders()
        {
            var patientId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var patient = _patientRepository.GetById(patientId);
            var orders = _orderRepository.GetOrdersByPatientId(patientId);

            var viewModel = orders.Select(o => new OrderViewModel
            {
                Id = o.Id,
                Description = o.Description,
                Duration = o.Duration,
                Address = o.Address,
                Status = o.Status,
                PatientAge = o.PatientAge,
                OrderDate = o.OrderDate,
                NurseId = o.NurseId,
                PatientId = o.PatientId,
                Gender = o.Gender
            }).ToList();

            ViewBag.PatientName = patient.FullName;
            ViewBag.PatientId = patientId;

            return View("/Views/User/PatientOrders.cshtml", viewModel);
        }

        [Authorize(Roles = "Patient")]
        [HttpGet("EditOrder/{id:int}")]
        public IActionResult EditOrder(int id)
        {
            var order = _orderRepository.GetById(id);
            var patientId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (order == null || order.PatientId != patientId)
                return NotFound();

            var viewModel = new OrderViewModel
            {
                Id = order.Id,
                Description = order.Description,
                Duration = order.Duration,
                Address = order.Address,
                Status = order.Status,
                PatientAge = order.PatientAge,
                OrderDate = order.OrderDate,
                NurseId = order.NurseId,
                PatientId = order.PatientId,
                Gender = order.Gender
            };

            ViewBag.Genders = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Text = "Male", Value = "Male" },
                new SelectListItem { Text = "Female", Value = "Female" }
            }, "Value", "Text");

            return View("/Views/User/EditOrder.cshtml", viewModel);
        }

        [Authorize(Roles = "Patient")]
        [HttpPost("EditOrder/{id:int}")]
        public IActionResult EditOrder(int id, [FromForm] OrderViewModel vm)
        {
            var patientId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var existingOrder = _orderRepository.GetById(id);

            if (existingOrder == null || existingOrder.PatientId != patientId)
                return NotFound();

            if (ModelState.IsValid)
            {
                // Check for conflicts with the same nurse (excluding the current order)
                var allOrders = _orderRepository.GetAll();
                var conflictOrder = allOrders
                    .Where(o => o.Id != id && 
                                o.NurseId == vm.NurseId &&
                                ((vm.OrderDate < o.OrderDate.AddHours(o.Duration)) && 
                                  (o.OrderDate < vm.OrderDate.AddHours(vm.Duration))))
                    .FirstOrDefault();

                if (conflictOrder != null)
                {
                    var conflictEnd = conflictOrder.OrderDate.AddHours(conflictOrder.Duration);
                    ModelState.AddModelError("", $"لا يمكن حجز هذه الخدمة في هذا الوقت. ({conflictOrder.OrderDate:HH:mm} - {conflictEnd:HH:mm})");
                    ViewBag.Genders = new SelectList(new List<SelectListItem>
                    {
                        new SelectListItem { Text = "Male", Value = "Male" },
                        new SelectListItem { Text = "Female", Value = "Female" }
                    }, "Value", "Text");
                    return View("/Views/User/EditOrder.cshtml", vm);
                }

                existingOrder.Description = vm.Description;
                existingOrder.Duration = vm.Duration;
                existingOrder.Address = vm.Address;
                existingOrder.PatientAge = vm.PatientAge;
                existingOrder.OrderDate = vm.OrderDate;
                existingOrder.Gender = vm.Gender;
                existingOrder.Status = OrderStatus.Pending; // Reset to pending when edited

                _orderRepository.Update(existingOrder);
                TempData["SuccessMessage"] = "Order updated successfully!";
                return RedirectToAction(nameof(PatientOrders));
            }

            ViewBag.Genders = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Text = "Male", Value = "Male" },
                new SelectListItem { Text = "Female", Value = "Female" }
            }, "Value", "Text");

            return View("/Views/User/EditOrder.cshtml", vm);
        }

        [Authorize(Roles = "Patient")]
        [HttpPost("DeleteOrder/{id:int}")]
        public IActionResult DeleteOrder(int id)
        {
            var patientId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var order = _orderRepository.GetById(id);

            if (order == null || order.PatientId != patientId)
                return NotFound();

            _orderRepository.Delete(order);
            TempData["SuccessMessage"] = "Order deleted successfully!";
            return RedirectToAction(nameof(PatientOrders));
        }

    }
}
