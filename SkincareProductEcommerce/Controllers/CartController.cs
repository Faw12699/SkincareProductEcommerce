using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkincareProductEcommerce.Data;
using SkincareProductEcommerce.Models;
using SkincareProductEcommerce.Models.ViewModels;
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
            };

            foreach (var item in shoppingCartVM.ShoppingCartList)
            {
                shoppingCartVM.totalPrice += Convert.ToDouble(item.Product.Price * item.Count);
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
