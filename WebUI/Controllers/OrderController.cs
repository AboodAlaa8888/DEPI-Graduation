using Core.Models;
using Core.RepositoryInterfaces;
using Microsoft.AspNetCore.Authorization; // Required for [Authorize]
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;
using WebUI.ViewModels;

namespace WebUI.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepo;
        private readonly INurseRepository _nurseRepo;
        private readonly IPatientRepository _patientRepo;

        public OrderController(IOrderRepository orderRepo, INurseRepository nurseRepo, IPatientRepository patientRepo)
        {
            _orderRepo = orderRepo;
            _nurseRepo = nurseRepo;
            _patientRepo = patientRepo;
        }

        // GET: /Order/Checkout?nurseId=5
        [HttpGet]
        public async Task<IActionResult> Checkout(int nurseId)
        {
            var nurse = await _nurseRepo.GetByIdAsync(nurseId);
            if (nurse == null) return NotFound();

            ViewBag.NurseName = nurse.FullName;
            ViewBag.NurseRate = 50;

            var model = new OrderViewModel
            {
                NurseId = nurseId,
                OrderDate = DateTime.Now.AddDays(1),
                Duration = 1
            };

            return View(model);
        }

        // POST: Creates order (without payment - payment happens after confirmation)
        [HttpPost]
        public async Task<IActionResult> CreateCheckoutSession(OrderViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToAction("Login", "Account");

            model.PatientId = userId;

            if (!ModelState.IsValid)
            {
                var nurseInfo = await _nurseRepo.GetByIdAsync(model.NurseId);
                ViewBag.NurseName = nurseInfo?.FullName ?? "Nurse";
                return View("Checkout", model);
            }

            var order = new Order
            {
                NurseId = model.NurseId,
                PatientId = userId,
                Duration = model.Duration,
                OrderDate = model.OrderDate,
                Address = model.Address,
                Description = model.Description,
                Status = OrderStatus.Pending,
                PatientAge = model.PatientAge,
                Gender = model.Gender
            };

            await _orderRepo.AddAsync(order);

            TempData["SuccessMessage"] = "Order created successfully! Please wait for the nurse to confirm your order.";
            return RedirectToAction("MyOrders");
        }

        // GET: Initiate Stripe Payment for confirmed orders
        [HttpGet]
        public async Task<IActionResult> PayOrder(int orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToAction("Login", "Account");

            var order = await _orderRepo.GetByIdAsync(orderId);
            if (order == null) return NotFound();

            // Verify the order belongs to the current user
            if (order.PatientId != userId)
            {
                return Forbid();
            }

            // Only allow payment for confirmed orders
            if (order.Status != OrderStatus.Confirmed)
            {
                TempData["ErrorMessage"] = "This order is not confirmed yet. Please wait for the nurse to confirm your order.";
                return RedirectToAction("MyOrders");
            }

            // Check if already paid
            if (order.Status == OrderStatus.Approved)
            {
                TempData["ErrorMessage"] = "This order has already been paid.";
                return RedirectToAction("MyOrders");
            }

            var nurse = await _nurseRepo.GetByIdAsync(order.NurseId);
            if (nurse == null) return NotFound();

            decimal hourlyRate = 50.00m;
            decimal totalAmount = hourlyRate * (decimal)order.Duration;

            var domain = $"{Request.Scheme}://{Request.Host}";

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(totalAmount * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = $"Session with {nurse.FullName}",
                                Description = $"{order.Description} - Date: {order.OrderDate.ToShortDateString()}",
                            },
                        },
                        Quantity = 1,
                    },
                },
                Mode = "payment",
                SuccessUrl = domain + $"/Order/PaymentSuccess?orderId={order.Id}",
                CancelUrl = domain + "/Order/PaymentCancel",
            };

            var service = new SessionService();
            Session session = service.Create(options);

            return Redirect(session.Url);
        }

        public async Task<IActionResult> PaymentSuccess(int orderId)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);
            if (order != null)
            {
                order.Status = OrderStatus.Approved;
                await _orderRepo.UpdateAsync(order);
            }
            return View();
        }

        public IActionResult PaymentCancel()
        {
            return View();
        }

        // GET: Patient History - Redirects to User/PatientOrders for consistency
        [HttpGet]
        public IActionResult MyOrders()
        {
            return RedirectToAction("PatientOrders", "User");
        }

        // --- NEW METHOD FOR NURSES ---
        [Authorize(Roles = "Nurse")]
        public IActionResult NurseOrders()
        {
            var username = User.Identity.Name;
            if (username == null) return RedirectToAction("Login", "Account");

            // Find the Nurse profile based on the logged-in username
            var nurse = _nurseRepo.GetByUserName(username);

            if (nurse == null)
            {
                return NotFound("Nurse profile not found for this user.");
            }

            var orders = _orderRepo.GetOrdersByNurseId(nurse.Id);
            return View(orders);
        }
    }
}