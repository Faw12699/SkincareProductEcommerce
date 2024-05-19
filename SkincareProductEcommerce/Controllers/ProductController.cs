using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SkincareProductEcommerce.Data;
using SkincareProductEcommerce.Models;
using SkincareProductEcommerce.Models.ViewModels;
using SkincareProductEcommerce.Utility;

namespace SkincareProductEcommerce.Controllers
{
    [Authorize(Roles = Role.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;

        }
        public IActionResult Index()
        {
            List<Product> productList = _db.Products.Include(p => p.Category).ToList();
            return View(productList);
        }
        public IActionResult Upsert(int? id)
        {
            ProductViewModel productVM = new ProductViewModel
            {
                categoryList = _db.Categories.ToList().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product(),
            };

            if (id == 0 || id == null) 
            { 
                return View(productVM); 
            }
            else
            {
                productVM.Product = _db.Products.Find(id);

                if (productVM.Product == null) { return NotFound(); }
                return View(productVM);
            }
        }
        [HttpPost]
        public IActionResult Upsert(ProductViewModel productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");

                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        var oldimagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldimagePath))
                        {
                            System.IO.File.Delete(oldimagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, filename), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVM.Product.ImageUrl = @"\images\product\" + filename;
                }
                if (productVM.Product.Id == 0)
                {
                    _db.Products.Add(productVM.Product);
                    TempData["success"] = "Product created successfuly!";
                }
                else
                {
                    _db.Products.Update(productVM.Product);
                    TempData["success"] = "Product updated successfuly!";
                }
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                productVM = new ProductViewModel
                {
                    categoryList = _db.Categories.ToList().Select(u=> new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString(),
                    })
                };
                return View();
            }
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _db.Products.Include(u => u.Category).ToList();
            return Json(new { data = objProductList });
        }

        //[HttpDelete]
        public IActionResult Delete(int id)
        {
            var productToBeDeleted = _db.Products.Find(id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting!" });
            }

            if (!string.IsNullOrEmpty(productToBeDeleted?.ImageUrl))
            {
                var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }
            _db.Products.Remove(productToBeDeleted);
            _db.SaveChanges();

            return Json(new { success = true, message = "Product successfully deleted!" });
        }
        #endregion
    }
}
