using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkincareProductEcommerce.Data;
using SkincareProductEcommerce.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace SkincareProductEcommerce.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            List<Product> productList = _db.Products.Include(u => u.Category).ToList();
            return View(productList);
        }
        public IActionResult Details(int id)
        {
            ShoppingCart cart = new ShoppingCart
            {
                Product = _db.Products.Include(u => u.Category).FirstOrDefault(u => u.Id == id),
                Count = 1,
                ProductId = id,
            };
            //Product? product = _db.Products.Include(u => u.Category).FirstOrDefault(u => u.Id == id);
            //if (product == null) { return NotFound(); }
            return View(cart);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            shoppingCart.Id = 0;
            var claimsIdentiy = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentiy.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserId = userId;

            ShoppingCart? countFromDb = _db.ShoppingCarts.FirstOrDefault(u => u.ApplicationUserId == userId && u.ProductId == shoppingCart.ProductId);

            if (countFromDb != null)
            {
                // Update Count (Quantity)
                countFromDb.Count += shoppingCart.Count;
                _db.ShoppingCarts.Update(countFromDb);
            }
            else
            {
                //Add Count
                _db.ShoppingCarts.Add(shoppingCart);
            }
            _db.SaveChanges();

            return RedirectToAction("Index");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
