using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkincareProductEcommerce.Data;
using SkincareProductEcommerce.Models;
using System.Diagnostics;

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
            Product? product = _db.Products.Include(u => u.Category).FirstOrDefault(u => u.Id == id);
            if (product == null) { return NotFound(); }
            return View(product);
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
