using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkincareProductEcommerce.Data;
using SkincareProductEcommerce.Models;
using SkincareProductEcommerce.Models.ViewModels;
using SkincareProductEcommerce.Utility;
using System.Security.Claims;

namespace SkincareProductEcommerce.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
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
        [HttpPost]
        public IActionResult Summary(ShoppingCartViewModel shoppingCartVM)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            shoppingCartVM.ShoppingCartList = _db.ShoppingCarts.Include(u => u.Product).Where(u => u.ApplicationUserId == userId).ToList();

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
            return View(shoppingCartVM);
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

            if (cartFromDb?.Count <= 1)
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
