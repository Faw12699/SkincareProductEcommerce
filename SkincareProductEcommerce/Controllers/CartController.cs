using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkincareProductEcommerce.Data;
using SkincareProductEcommerce.Models;
using SkincareProductEcommerce.Models.ViewModels;
using SkincareProductEcommerce.Utility;
using Stripe.Checkout;
using System;
using System.Security.Claims;

namespace SkincareProductEcommerce.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        [BindProperty]
        public ShoppingCartViewModel shoppingCartVM { get; set; }
        public CartController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            shoppingCartVM = new ShoppingCartViewModel
            {
                ShoppingCartList = _db.ShoppingCarts.Include(u => u.Product).Where(u=>u.ApplicationUserId==userId).ToList(),   
                OrderHeader = new OrderHeader(),
            };

            foreach (var item in shoppingCartVM.ShoppingCartList)
            {
                shoppingCartVM.OrderHeader.OrderTotal += Convert.ToDouble(item.Product.Price * item.Count);
            }
            return View(shoppingCartVM);
        }
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            shoppingCartVM = new ShoppingCartViewModel
            {
                ShoppingCartList = _db.ShoppingCarts.Include(u => u.Product).Where(u => u.ApplicationUserId == userId).ToList(),
                OrderHeader = new(),
            };

            shoppingCartVM.OrderHeader.ApplicationUser = _db.ApplicationUsers.FirstOrDefault(u => u.Id == userId);

            shoppingCartVM.OrderHeader.Name = shoppingCartVM.OrderHeader.ApplicationUser.Name;
            shoppingCartVM.OrderHeader.PhoneNumber = shoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            shoppingCartVM.OrderHeader.StreetAddress = shoppingCartVM.OrderHeader.ApplicationUser.streetAddress;
            shoppingCartVM.OrderHeader.City = shoppingCartVM.OrderHeader.ApplicationUser.City;
            shoppingCartVM.OrderHeader.State = shoppingCartVM.OrderHeader.ApplicationUser.State;
            shoppingCartVM.OrderHeader.PostalCode = shoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

            foreach (var item in shoppingCartVM.ShoppingCartList)
            {
                shoppingCartVM.OrderHeader.OrderTotal += Convert.ToDouble(item.Product.Price * item.Count);
            }
            return View(shoppingCartVM);
        }
        [HttpPost, ActionName("Summary")]
        public IActionResult SummaryPost()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            shoppingCartVM.ShoppingCartList = _db.ShoppingCarts.Include(u => u.Product).Where(u => u.ApplicationUserId == userId).ToList();

            if (ModelState.IsValid)
            {
                shoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
                shoppingCartVM.OrderHeader.ApplicationUserId = userId;

                shoppingCartVM.OrderHeader.ApplicationUser = _db.ApplicationUsers.FirstOrDefault(u => u.Id == userId);

                foreach (var item in shoppingCartVM.ShoppingCartList)
                {
                    shoppingCartVM.OrderHeader.OrderTotal += Convert.ToDouble(item.Product.Price * item.Count);
                }

                shoppingCartVM.OrderHeader.OrderStatus = Status.StatusPending;
                shoppingCartVM.OrderHeader.PayementStatus = Status.PaymentStatusPending;

                _db.OrderHeaders.Add(shoppingCartVM.OrderHeader);
                _db.SaveChanges();

                foreach (var item in shoppingCartVM.ShoppingCartList)
                {
                    OrderDetails orderDetails = new()
                    {
                        ProductId = item.ProductId,
                        OrderHeaderId = shoppingCartVM.OrderHeader.Id,
                        Price = item.Product.Price,
                        Count = item.Count,
                    };
                    _db.OrderDetails.Add(orderDetails);
                    _db.SaveChanges();
                }

                var domain = Request.Scheme + "://" + Request.Host.Value + "/";
                var options = new SessionCreateOptions
                {
                    SuccessUrl = domain + $"cart/OrderConfirmation?id={shoppingCartVM.OrderHeader.Id}",
                    CancelUrl = domain + "cart/index",
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };

                foreach (var item in shoppingCartVM.ShoppingCartList)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Product.Price * 100), // $20.50 => 2050
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Name
                            }
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionLineItem);
                }

                var service = new SessionService();
                Session session = service.Create(options);
                UpdateStripePaymentID(shoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }
            return View(shoppingCartVM);
        }
        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderheader = _db.OrderHeaders.Include(u => u.ApplicationUser).FirstOrDefault(u => u.Id == id);

            var service = new SessionService();
            Session session = service.Get(orderheader.SessionId);

            if (session.PaymentStatus.ToLower() == "paid")
            {
                UpdateStripePaymentID(id, session.Id, session.PaymentIntentId);
                UpdateStatus(id, Status.StatusApproved, Status.PaymentStatusApproved);
            }

            List<ShoppingCart> shoppingCart = _db.ShoppingCarts.Where(u => u.ApplicationUserId == orderheader.ApplicationUserId).ToList();
            _db.ShoppingCarts.RemoveRange(shoppingCart);
            _db.SaveChanges();

            return View(id);
        }
        // Updating Status of Order and Payment 
        public void UpdateStatus(int id, string orderStatus, string paymentStatus)
        {
            var orderFromDb = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
            if (orderFromDb !=null)
            {
                orderFromDb.OrderStatus = orderStatus;
                if (!string.IsNullOrEmpty(paymentStatus))
                {
                    orderFromDb.PayementStatus = paymentStatus;

                    _db.OrderHeaders.Update(orderFromDb);
                    _db.SaveChanges();
                }
            }
        }
        public void UpdateStripePaymentID(int id, string sessionId, string paymentIntentId)
        {
            var orderFromDb = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
            if (!string.IsNullOrEmpty(sessionId)) 
            {
                orderFromDb.SessionId = sessionId;
            }
            if (!string.IsNullOrEmpty(paymentIntentId))
            {
                orderFromDb.PaymentIntentId = paymentIntentId;
                orderFromDb.PaymentDate = DateTime.Now;
            }
            _db.OrderHeaders.Update(orderFromDb);
            _db.SaveChanges();
        }
        public IActionResult Plus(int cartId)
        {
            var cartFromDb = _db.ShoppingCarts.FirstOrDefault(u => u.Id == cartId);
            if (cartFromDb == null) { return NotFound(); }

            cartFromDb.Count += 1;
            _db.ShoppingCarts.Update(cartFromDb);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }
        public IActionResult Minus(int cartId)
        {
            var cartFromDb = _db.ShoppingCarts.FirstOrDefault(u => u.Id == cartId);
            if (cartFromDb == null) { return NotFound(); }

            cartFromDb.Count -= 1;

            if (cartFromDb?.Count < 1)
            {
                _db.ShoppingCarts.Remove(cartFromDb);
            }
            else
            {
                _db.ShoppingCarts.Update(cartFromDb);
            }
            _db.SaveChanges(); 
            return RedirectToAction("Index");
        }
        public IActionResult Remove(int cartId)
        {
            var cartFromDb = _db.ShoppingCarts.FirstOrDefault(u => u.Id == cartId);
            if (cartFromDb == null) { return NotFound(); }

            _db.ShoppingCarts.Remove(cartFromDb);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
