using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkincareProductEcommerce.Data;
using SkincareProductEcommerce.Models;
using SkincareProductEcommerce.Models.ViewModels;
using SkincareProductEcommerce.Utility;
using Stripe;
using Stripe.Climate;
using System.Security.Claims;

namespace SkincareProductEcommerce.Controllers
{
    public class OrderController : Controller
    {

        private readonly ApplicationDbContext _db;
        public OrderController(ApplicationDbContext db)
        {
            _db = db;
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Details(int id)
        {
            OrderViewModel orderViewModel = new()
            {
                OrderHeader = _db.OrderHeaders.Include(u => u.ApplicationUser).FirstOrDefault(u => u.Id == id),
                orderDetailsList = _db.OrderDetails.Include(u => u.Product).Where(u => u.Id == id).ToList(),
            };
            return View(orderViewModel);
        }
        [HttpPost]
        //[Authorize(Role.Role_Admin)]
        public IActionResult UpdateOrderDetails(OrderViewModel OrderVM)
        {
            var orderHeaderFromDb = _db.OrderHeaders.FirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id);
            orderHeaderFromDb.Name = OrderVM.OrderHeader.Name;
            orderHeaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
            orderHeaderFromDb.City = OrderVM.OrderHeader.City;
            orderHeaderFromDb.State = OrderVM.OrderHeader.State;
            orderHeaderFromDb.PostalCode = OrderVM.OrderHeader.PostalCode;
            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier))
            {
                orderHeaderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
            }
            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.TrackingNumber))
            {
                orderHeaderFromDb.Carrier = OrderVM.OrderHeader.TrackingNumber;
            }
            _db.OrderHeaders.Update(orderHeaderFromDb);
            _db.SaveChanges();

            TempData["Success"] = "Order Details Updated Successfully.";

            return RedirectToAction("Index", new { id = orderHeaderFromDb.Id });
        }
        
        [HttpPost]
        public IActionResult StartProcessing(OrderViewModel OrderVM)
        {
            UpdateStatus(OrderVM.OrderHeader.Id, Status.StatusInProcess);
            TempData["Success"] = "Order Details Updated Successfully.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ShipOrder(OrderViewModel OrderVM)
        {

            var orderHeader = _db.OrderHeaders.FirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id);
            orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
            orderHeader.OrderStatus = Status.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;

            _db.OrderHeaders.Update(orderHeader);
            _db.SaveChanges();
            TempData["Success"] = "Order Shipped Successfully.";
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult CancelOrder(OrderViewModel OrderVM)
        {

            var orderHeader = _db.OrderHeaders.FirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id);

            if (orderHeader.PayementStatus == Status.PaymentStatusApproved)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId
                };

                var service = new RefundService();
                Refund refund = service.Create(options);

                UpdateStatus(orderHeader.Id, Status.StatusCancelled, Status.StatusRefunded);
            }
            else
            {
                UpdateStatus(orderHeader.Id, Status.StatusCancelled, Status.StatusCancelled);
            }
            _db.SaveChanges();
            TempData["Success"] = "Order Cancelled Successfully.";
            return RedirectToAction("Index");

        }

        public void UpdateStatus(int id, string orderStatus, string paymentStatus = null)
        {
            var orderFromDb = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
            if (orderFromDb != null)
            {
                orderFromDb.OrderStatus = orderStatus;
                if (!string.IsNullOrEmpty(paymentStatus))
                {
                    orderFromDb.PayementStatus = paymentStatus;
                }
            }
            _db.OrderHeaders.Update(orderFromDb);
            _db.SaveChanges();
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<OrderHeader> objOrderHeader;

            if (User.IsInRole(Role.Role_Admin))
            {
                objOrderHeader = _db.OrderHeaders.Include(u => u.ApplicationUser).ToList();
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                objOrderHeader = _db.OrderHeaders.Include(u => u.ApplicationUser).Where(u => u.ApplicationUserId == userId).ToList();
            }
            
            return Json(new { data = objOrderHeader });
        }

        #endregion
    }
}
