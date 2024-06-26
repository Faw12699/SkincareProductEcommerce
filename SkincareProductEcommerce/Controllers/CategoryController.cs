﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SkincareProductEcommerce.Data;
using SkincareProductEcommerce.Models;
using SkincareProductEcommerce.Utility;

namespace SkincareProductEcommerce.Controllers
{
    [Authorize(Roles = Role.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            // list of categories
            List<Category> categoryList=_db.Categories.ToList();
            return View(categoryList);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category category) 
        {
            if (ModelState.IsValid)
            {
                _db.Categories.Add(category);
                _db.SaveChanges();
                TempData["success"] = "Category created successfully!";
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0) { return NotFound(); }

            Category? categoryFormDb = _db.Categories.Find(id);

            if (categoryFormDb == null) { return NotFound(); }

            return View(categoryFormDb);
        }
        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _db.Categories.Update(category);
                _db.SaveChanges();
                TempData["success"] = "Category updated successfully!";
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0) { return NotFound(); }

            Category? categoryFromDb= _db.Categories.Find(id);

            if (categoryFromDb == null) { return NotFound(); }

            return View(categoryFromDb);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id) 
        {
            Category? category = _db.Categories.Find(id);

            if (category == null) { return NotFound(); }
      
            _db.Categories.Remove(category);
            _db.SaveChanges();
            TempData["success"] = "Category deleted successfully!";
            return RedirectToAction("Index");
        }
    }
}
